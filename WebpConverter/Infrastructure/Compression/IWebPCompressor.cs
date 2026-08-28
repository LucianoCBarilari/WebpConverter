namespace WebpConverter.Infrastructure.Compression;

public interface IWebPCompressor
{
    Task<Stream> ConvertToWebpAsync(Stream input, int quality, CancellationToken cancellationToken = default);
}
