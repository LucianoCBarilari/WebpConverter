using MediaProcessing.Common.Results;

namespace MediaProcessing.Feature.ProcessImage;

public interface IImageProcessingService
{
    Task<ResultMedia<string>> ImageProcessAsync(ImageProcessingRequest request);
}
