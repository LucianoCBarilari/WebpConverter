using MediaProcessorLibrary.Application.Interfaces;
using MediaProcessorLibrary.Domain.Enums;
using SixLabors.ImageSharp;

namespace MediaProcessorLibrary.Infrastructure.Imaging
{
    public class ImageValidator : IImageValidator
    {
            public ImageValidationResult Validate(
                                                    Stream imageStream,
                                                    int maxWidth,
                                                    int maxHeight,
                                                    long maxSizeBytes)
            {
                if (imageStream == null || imageStream.Length == 0)
                    return ImageValidationResult.Empty;

                if (imageStream.Length > maxSizeBytes)
                    return ImageValidationResult.TooLarge;

                try
                {
                    if (imageStream.CanSeek)
                        imageStream.Position = 0;

                    var info = Image.Identify(imageStream);

                    if (info == null)
                        return ImageValidationResult.Corrupted;

                    if (info.Width > maxWidth || info.Height > maxHeight)
                        return ImageValidationResult.InvalidDimensions;

                    return ImageValidationResult.Valid;
                }
                catch
                {
                    return ImageValidationResult.Corrupted;
                }
            }

    }
}
