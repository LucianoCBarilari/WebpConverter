using MediaProcessorLibrary.Application.Results;

namespace MediaProcessorLibrary.Application.Interfaces
{
    public interface IFormatCompress
    {
        Task<Result<Stream>> ConvertToWebpAsync(Stream input, int quality);
    }

}
