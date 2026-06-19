using MediaProcessorLibrary.Common.Results;

namespace MediaProcessorLibrary.Feature.ProcessImage;

public interface IImageProcessingService
{
    Task<ResultMedia<string>> ImageProcessAsync(ImageProcessingRequest request);
}
