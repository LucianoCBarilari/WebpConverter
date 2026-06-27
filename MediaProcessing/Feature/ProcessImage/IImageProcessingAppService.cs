using MediaProcessing.Common.Results;

namespace MediaProcessing.Feature.ProcessImage
{
    public interface IImageProcessingAppService
    {
        Task<ResultMedia<string>> ProcessImage(IFormFile file, int maxFileSize, string fileName, Guid imageSizeId, int quality = 80);
    }
}
