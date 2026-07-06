namespace WebpConverter.Infrastructure.Compression;

public interface IWebPCompressor
{
    Task<Stream> ConvertToWebpAsync(Stream input, int quality = 80);
}
