using Microsoft.Extensions.Options;
using SkiaSharp;
using WebpConverter.Common.Enums;
using WebpConverter.Common.Options;
using WebpConverter.Common.Results;
using WebpConverter.Feature.DeleteImage;
using WebpConverter.Infrastructure.Compression;
using WebpConverter.Infrastructure.FileSystem;

namespace WebpConverter.Feature.ProcessImage;

/// <summary>
/// Unified orchestrator/handler for the process image vertical slice.
/// Consolidates file validation, WebP compression, storage persistence,
/// cleanup of previous assets, and public path mapping.
/// </summary>
public class ProcessImageHandler(
        IOptions<AppOptions> options,
        IWebPCompressor webPCompressor,
        IFileService fileService,
        IDeleteImageHandler deleteImageHandler)
{
    private readonly AppOptions _options = options.Value;
    private readonly IWebPCompressor _webPCompressor = webPCompressor;
    private readonly IFileService _fileService = fileService;
    private readonly IDeleteImageHandler _deleteImageHandler = deleteImageHandler;



    /// <summary>
    /// Executes the process image flow: validation, conversion, saving, and cleanup.
    /// </summary>
    /// <param name="command">The payload containing the image stream and file metadata.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A ResultMedia containing the public URL path if successful, or an error code.</returns>
    public async Task<ResultMedia<string>> HandleAsync(ImageToProcess command, CancellationToken cancellationToken = default)
    {
        if (command?.ImageStream == null)
            return ResultMedia<string>.Fail(ErrorCode.InvalidStream);

        // 1. Validate stream size constraints
        var sizeValidation = ImageValidator.ValidateSize(command.ImageStream, _options.MaxFileSizeBytes);
        if (!sizeValidation.IsSuccess)
            return ResultMedia<string>.Fail(sizeValidation.Error ?? ErrorCode.ImageTooLarge);

        // 2. Validate file integrity and structure
        var imageValidation = ImageValidator.ValidateImage(command.ImageStream);
        if (!imageValidation.IsSuccess)
            return ResultMedia<string>.Fail(imageValidation.Error ?? ErrorCode.InvalidImage);

        if (command.ImageStream.CanSeek)
            command.ImageStream.Position = 0;

        // 3. Detect format to decide processing path
        var format = ImageValidator.DetectFormat(command.ImageStream);
        bool isGif = format == SKEncodedImageFormat.Gif;

        // 4. Generate secure filename
        string baseName = Path.GetFileNameWithoutExtension(command.FileName);

        var generatedName = _fileService.GenerateFileName(baseName);
        if (string.IsNullOrEmpty(generatedName))
            return ResultMedia<string>.Fail(ErrorCode.FileNameEmpty);

        var storagePathFull = Path.GetFullPath(_options.StoragePath);
        var combinedPath = Path.GetFullPath(Path.Combine(_options.StoragePath, command.Subfolder ?? ""));

        if (!combinedPath.StartsWith(storagePathFull, StringComparison.OrdinalIgnoreCase))
        {
            return ResultMedia<string>.Fail(ErrorCode.Unauthorized);
        }
        var storageFolder = combinedPath;

        var publicUrlFolder = string.IsNullOrWhiteSpace(command.Subfolder)
            ? _options.PublicUrlPath
            : $"{_options.PublicUrlPath}/{command.Subfolder}";

        string extension;
        Stream streamToSave;

        if (isGif)
        {
            // GIF path: pass-through, preserve animation, no compression
            extension = "gif";
            streamToSave = command.ImageStream;
        }
        else
        {
            // WebP path: compress and convert
            extension = "webp";
            streamToSave = await _webPCompressor.ConvertToWebpAsync(
                command.ImageStream,
                _options.ImageCompressionQuality,
                cancellationToken);
        }

        var fullPath = Path.Combine(storageFolder, $"{generatedName}.{extension}");

        // 5. Save to physical storage
        var saveResult = await _fileService.SaveAsync(streamToSave, fullPath, cancellationToken);

        if (!isGif)
            streamToSave.Dispose();

        if (!saveResult.IsSuccess)
            return ResultMedia<string>.Fail(saveResult.Error ?? ErrorCode.SaveFailed);

        // 6. Clean up previous image if requested
        if (!string.IsNullOrWhiteSpace(command.PreviousFileName))
        {
            _deleteImageHandler.Delete(storageFolder, command.PreviousFileName);
        }

        // 7. Generate and return the public web path
        var publicPath = $"{publicUrlFolder}/{generatedName}.{extension}";
        return ResultMedia<string>.Ok(publicPath, Operation.Saved);
    }
}

