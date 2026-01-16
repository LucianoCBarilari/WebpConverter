using MediaProcessorLibrary.Application.ImageProcessing;
using MediaProcessorLibrary.Application.Results;

namespace MediaProcessorLibrary.Application.UseCases
{
    public interface IImageProcessingService
    {
        Task<Result<string>> ImageProcessAsync(ImageProcessingRequest request);
    }
}
