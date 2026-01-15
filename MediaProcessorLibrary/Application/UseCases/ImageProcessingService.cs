using MediaProcessorLibrary.Application.ImageProcessing;
using MediaProcessorLibrary.Application.Interfaces;
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
        public async Task<ImageProcessingResult> ImageProcessAsync(ImageProcessingRequest request)
        {
            if (request?.ImageStream == null)
                return ImageProcessingResult.InvalidInput;

            var validation = _ImageValidator.Validate(
                request.ImageStream,
                request.MaxWidth,
                request.MaxHeight,
                request.MaxSizeBytes);

            if (validation != ImageValidationResult.Valid)
                return ImageProcessingResult.InvalidImage;

            if (request.ImageStream.CanSeek)
                request.ImageStream.Position = 0;

            using  var compressedStream = await _FormatCompress.ConvertToWebpAsync(
                request.ImageStream,
                request.Quality);

            _Directory.FolderExist(request.OutputDirectory);

            var generatedName = _Helper.GenerateFileName(request.OutputFileName);

            var fullPath = Path.Combine(request.OutputDirectory,$"{generatedName}.webp");

            await _File.SaveAsync(compressedStream, fullPath);

            return ImageProcessingResult.Success;
        }

    }
}
