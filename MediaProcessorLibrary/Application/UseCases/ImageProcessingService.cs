using MediaProcessorLibrary.Application.ImageProcessing;
using MediaProcessorLibrary.Application.Interfaces;
using MediaProcessorLibrary.Application.Results;
using MediaProcessorLibrary.Common.Helpers;
using MediaProcessorLibrary.Domain.Enums;

namespace MediaProcessorLibrary.Application.UseCases
{
    public class ImageProcessingService : IImageProcessingService
    {
        private readonly IDirectoryService _Directory;
        private readonly IFileService _File;
        private readonly IImageValidator _ImageValidator;
        private readonly IFormatCompress _FormatCompress;
        private readonly IHelpers _Helper;

        public ImageProcessingService(
            IDirectoryService directory,
            IFileService file,
            IImageValidator imageValidator,
            IFormatCompress formatCompress,
            IHelpers helper
            )
        {
            _Directory = directory;
            _File = file;
            _ImageValidator = imageValidator;
            _FormatCompress = formatCompress;
            _Helper = helper;
        }
        public async Task<Result<string>> ImageProcessAsync(ImageProcessingRequest request)
        {
            if (request?.ImageStream == null)
                return Result<string>.Fail(ErrorCode.InvalidStream);

            // Validación de imagen
            var validationResult = _ImageValidator.Validate(
                request.ImageStream,
                request.MaxWidth,
                request.MaxHeight,
                request.MaxSizeBytes);

            if (!validationResult.IsSuccess)
                return Result<string>.Fail(validationResult.Error ?? ErrorCode.InvalidImage);

            try
            {
                // Reset del stream si es posible
                if (request.ImageStream.CanSeek)
                    request.ImageStream.Position = 0;

                // Convertir a WebP
                var convertResult = await _FormatCompress.ConvertToWebpAsync(
                    request.ImageStream,
                    request.Quality);

                if (!convertResult.IsSuccess || convertResult.Value == null)
                    return Result<string>.Fail(convertResult.Error ?? ErrorCode.CorruptedImage);

                var compressedStream = convertResult.Value;

                // Crear carpeta si no existe
                if (!_Directory.FolderExist(request.OutputDirectory))
                {
                    var createDirectoryResult = _Directory.CreateFolder(Path.GetDirectoryName(request.OutputDirectory) ?? "", Path.GetFileName(request.OutputDirectory));
                    if (!createDirectoryResult.IsSuccess)
                        return Result<string>.Fail(createDirectoryResult.Error ?? ErrorCode.Unexpected);
                }

                // Generar nombre de archivo
                var generatedName = _Helper.GenerateFileName(request.OutputFileName);
                var fullPath = Path.Combine(request.OutputDirectory, $"{generatedName}.webp");

                // Guardar archivo
                var saveResult = await _File.SaveAsync(compressedStream, fullPath);

                if (!saveResult.IsSuccess)
                    return Result<string>.Fail(saveResult.Error ?? ErrorCode.Unexpected);

                return Result<string>.Ok(fullPath, Operation.Saved);
            }
            catch (UnauthorizedAccessException)
            {
                return Result<string>.Fail(ErrorCode.Unauthorized);
            }
            catch (IOException)
            {
                return Result<string>.Fail(ErrorCode.IOError);
            }
            catch
            {
                return Result<string>.Fail(ErrorCode.Unexpected);
            }
        }


    }
}
