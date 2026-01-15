namespace MediaProcessorLibrary.Application.Interfaces
{
    public interface IFormatCompress
    {
        Task<Stream> ConvertToWebpAsync(Stream input, int quality);
    }

}
