
using SkiaSharp;
namespace WebpConverter.Infrastructure.Compression;

public class WebPCompressor
{
    public async Task<Stream> ConvertToWebpAsync(Stream input, int quality)
    {       
        return await Task.Run(() =>
        {
            using var bitmap = SKBitmap.Decode(input);
            
            var output = new MemoryStream();            
            
            bitmap.Encode(output, SKEncodedImageFormat.Webp, quality);
            
            output.Position = 0; 
            
            return (Stream)output;
        });
    }
}



