using MediaProcessorLibrary.Common.Results;

namespace MediaProcessorLibrary.Feature.ProcessImage
{
    public interface IImageProcessingAppService
    {
        Task<ResultMedia<string>> ProcessImage(IFormFile file, int maxFileSize, string fileName, Guid imageSizeId, int quality = 80);
    }
}
