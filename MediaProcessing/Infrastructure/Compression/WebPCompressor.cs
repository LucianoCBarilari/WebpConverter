using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;

namespace MediaProcessing.Infrastructure.Compression;
// TODO: [Dependency/Design] Implement WebP and optimize image processing settings.
/* 
 * Implementation & Architectural Notes for Image Processing:
 * 
 * 1. Dependency Management (ACTION REQUIRED): Ensure dependency on SixLabors.ImageSharp >= 3.1.12 to guarantee WebP support functionality.
 * 2. Optimization Strategy (REVIEW): Consult official documentation to balance image quality and file size via configuration settings. A review of lossless compression options should be performed where image fidelity is critical.
 * 3. Licensing Review (CRITICAL RISK): Since this tool targets open-source usage, conduct a thorough audit of the SixLabors package license to ensure compliance and identify any restricted clauses.
 */
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



