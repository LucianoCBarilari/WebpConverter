using MediaProcessorLibrary.Application.Results;

namespace MediaProcessorLibrary.Application.Interfaces
{
    public interface IImageValidator
    {
        Result Validate(
            Stream imageStream,
            int maxWidth,
            int maxHeight,
            long maxSizeBytes
        );
    }

}
