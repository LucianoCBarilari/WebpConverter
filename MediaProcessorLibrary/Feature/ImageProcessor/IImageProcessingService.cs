namespace MediaProcessorLibrary;

public interface IImageProcessingService
{
    public Task<ResultMedia<string>> ImageProcessAsync(ImageProcessingRequest request);
    public ResultMedia Delete(string folderPath, string fileName);
}
