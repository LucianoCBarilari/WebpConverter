namespace MediaProcessorLibrary
{
    public interface IImageProcessingAppService
    {
        Task<ResultMedia<string>> ProcessImage(IFormFile file, int maxFileSize, string fileName, Guid imageSizeId, int quality = 80);
    }
}
