using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;

namespace MediaProcessing.Infrastructure.Compression;
public class WebPCompressor
{
        public async Task<Stream> ConvertToWebpAsync(Stream input, int quality)
    {
        var decoderOptions = new DecoderOptions
        {
            Configuration = Configuration.Default,
            SkipMetadata = true
        };

        using Image image = await Image.LoadAsync(decoderOptions, input);

        var encoder = new WebpEncoder
        {
            Quality = quality,
            FileFormat = WebpFileFormatType.Lossy,
            Method = WebpEncodingMethod.Fastest
        };

        var output = new MemoryStream();
        await image.SaveAsync(output, encoder);

        output.Position = 0; 

        return output;
    }
}



