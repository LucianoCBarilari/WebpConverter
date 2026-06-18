using MediaProcessorLibrary.Common.Enums;
using MediaProcessorLibrary.Common.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediaProcessorLibrary.Feature.ProcessImage
{
    public class ImageProcessingAppService : IImageProcessingAppService
    {
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IConfiguration _configuration;
        //private readonly IImageSizeRepository _imageSizeRepository;

        private readonly string _fullOutputDirectory = string.Empty;
        private readonly string _publicImagePath = string.Empty;

        private readonly string? newImage = string.Empty;

        public ImageProcessingAppService(
            IImageProcessingService imageProcessingService,
            IConfiguration configuration)
        {
            _imageProcessingService = imageProcessingService;
            _configuration = configuration;
            //_imageSizeRepository = imageSizeRepository;
            _fullOutputDirectory = _configuration["FileStorageSettings:PhysicalImagePath"] ?? throw new InvalidOperationException("FileStorage");
            _publicImagePath = _configuration["FileStorageSettings:PublicImagePath"] ?? throw new InvalidOperationException("FileStorage");
        }

        public async Task<ResultMedia<string>> ProcessImage(IFormFile file, int maxFileSize, string fileName, Guid imageSizeId, int quality = 80)
        {
            /*int maxWidth = 1280;
            int maxHeight = 1920;

            var imageSize = await _imageSizeRepository.GetByIdAsync(imageSizeId);
            if (imageSize != null)
            {
                maxWidth = imageSize.ImageSizeWidth;
                maxHeight = imageSize.ImageSizeHeight;
            }*/

            if (string.IsNullOrWhiteSpace(_fullOutputDirectory))
            {
                return ResultMedia<string>.Fail(ErrorCode.Unexpected);
            }

            if (file.Length > maxFileSize)
            {
                return ResultMedia<string>.Fail(ErrorCode.ImageTooLarge);
            }

            using var browserStream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();

            await browserStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var request = new ImageProcessingRequest
            {
                ImageStream = memoryStream,
                OutputDirectory = _fullOutputDirectory,
                OutputFileName = fileName,
                MaxSizeBytes = maxFileSize,
                Quality = quality,
                MaxWidth = 0,//maxWidth,
                MaxHeight = 0//maxHeight
            };

            var Result = await _imageProcessingService.ImageProcessAsync(request);

            if (Result.IsSuccess && !string.IsNullOrWhiteSpace(Result.Value))
            {
                var currentFileName = Path.GetFileName(Result.Value);
                var publicPath = $"{_publicImagePath}/{currentFileName}";

                if (!string.IsNullOrWhiteSpace(newImage))
                {
                    _imageProcessingService.Delete(_fullOutputDirectory, Path.GetFileName(newImage));
                }

                return ResultMedia<string>.Ok(publicPath, Operation.Saved);
            }

            return ResultMedia<string>.Fail(Result.Error ?? ErrorCode.Unexpected);
        }
    }
