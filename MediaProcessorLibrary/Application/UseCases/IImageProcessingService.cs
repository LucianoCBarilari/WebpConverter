using MediaProcessorLibrary.Application.ImageProcessing;

namespace MediaProcessorLibrary.Application.UseCases
{
    public interface IImageProcessingService
    {
        public Task<ImageProcessingResult> ImageProcessAsync(ImageProcessingRequest request);
    }
}
