
using SkiaSharp;
namespace WebpConverter.Infrastructure.Compression;

public class WebPCompressor : IWebPCompressor
{
    public async Task<Stream> ConvertToWebpAsync(Stream input, int quality)
    {       
        return await Task.Run(() =>
        {
            using var bitmap = SKBitmap.Decode(input) ?? throw new InvalidOperationException("Failed to decode image");
            
            var output = new MemoryStream();            
            
            bitmap.Encode(output, SKEncodedImageFormat.Webp, quality);
            
            output.Position = 0; 
            
            return (Stream)output;
        });
    }
}



