using Microsoft.Extensions.Options;
using MediaProcessing.Common;
using MediaProcessing.Common.Enums;
using MediaProcessing.Common.Results;
using MediaProcessing.Feature.DeleteImage;
using MediaProcessing.Infrastructure.Compression;
using MediaProcessing.Infrastructure.FileSystem;

namespace MediaProcessing.Feature.ProcessImage;

/// <summary>
/// Unified orchestrator/handler for the process image vertical slice.
/// Consolidates file validation, WebP compression, storage persistence,
/// cleanup of previous assets, and public path mapping.
/// </summary>
public class ProcessImageHandler(
        IOptions<AppOptions> options,
        WebPCompressor webPCompressor,
        FileService fileService,
        DeleteImageHandler deleteImageHandler)
{
    private readonly AppOptions _options = options.Value;
    private readonly WebPCompressor _webPCompressor = webPCompressor;
    private readonly FileService _fileService = fileService;
    private readonly DeleteImageHandler _deleteImageHandler = deleteImageHandler;

    /// <summary>
    /// Executes the process image flow: validation, conversion, saving, and cleanup.
    /// </summary>
    /// <param name="command">The payload containing the image stream and file metadata.</param>
    /// <returns>A ResultMedia containing the public URL path if successful, or an error code.</returns>
    public async Task<ResultMedia<string>> HandleAsync(ImageToProcess command)
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

        // 3. Compress/Convert stream to WebP
        using var compressedStream = await _webPCompressor.ConvertToWebpAsync(
            command.ImageStream,
            _options.ImageCompressionQuality);

        // 4. Generate secure filename
        string baseName = Path.GetFileNameWithoutExtension(command.FileName);

        var generatedName = fileService.GenerateFileName(baseName);
        if (string.IsNullOrEmpty(generatedName))
            return ResultMedia<string>.Fail(ErrorCode.FileNameEmpty);

        var fullPath = Path.Combine(_options.StoragePath, $"{generatedName}.webp");

        // 5. Save to physical storage
        var saveResult = await _fileService.SaveAsync(compressedStream, fullPath);
        if (!saveResult.IsSuccess)
            return ResultMedia<string>.Fail(saveResult.Error ?? ErrorCode.SaveFailed);

        // 6. Clean up previous image if requested
        if (!string.IsNullOrWhiteSpace(command.PreviousFileName))
        {
            _deleteImageHandler.Delete(_options.StoragePath, command.PreviousFileName);
        }

        // 7. Generate and return the public web path
        var publicPath = $"{_options.PublicUrlPath}/{generatedName}.webp";
        return ResultMedia<string>.Ok(publicPath, Operation.Saved);
    }
}
