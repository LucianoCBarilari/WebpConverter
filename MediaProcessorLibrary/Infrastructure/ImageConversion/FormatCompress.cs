using MediaProcessorLibrary.Application.Interfaces;
using MediaProcessorLibrary.Application.Results;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;

namespace MediaProcessorLibrary.Infrastructure.ImageConversion
{
    public class FormatCompress : IFormatCompress
    {
        public async Task<Result<Stream>> ConvertToWebpAsync(Stream input, int quality)
        {
            try
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
                return Result<Stream>.Ok(output, Operation.Converted);
            }
            catch
            {
                return Result<Stream>.Fail(ErrorCode.CorruptedImage);
            }
        }


    }

}
