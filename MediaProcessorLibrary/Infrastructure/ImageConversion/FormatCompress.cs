using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp;
using MediaProcessorLibrary.Application.Interfaces;

namespace MediaProcessorLibrary.Infrastructure.ImageConversion
{
    public class FormatCompress : IFormatCompress
    {
        public async Task<Stream> ConvertToWebpAsync(Stream input, int quality)
        {
            if (input.CanSeek)
                input.Position = 0;

            using Image image = await Image.LoadAsync(input);

            var encoder = new WebpEncoder
            {
                Quality = quality,
                FileFormat = WebpFileFormatType.Lossy
            };

            var output = new MemoryStream();
            await image.SaveAsync(output, encoder);

            output.Position = 0; // MUY IMPORTANTE

            return output;
        }
    }

}
