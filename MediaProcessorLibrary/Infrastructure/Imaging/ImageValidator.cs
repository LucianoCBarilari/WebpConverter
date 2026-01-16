using MediaProcessorLibrary.Application.Interfaces;
using MediaProcessorLibrary.Application.Results;
using MediaProcessorLibrary.Domain.Enums;
using SixLabors.ImageSharp;

namespace MediaProcessorLibrary.Infrastructure.Imaging
{
    public class ImageValidator : IImageValidator
    {
        public Result Validate(
                                Stream imageStream,
                                int maxWidth,
                                int maxHeight,
                                long maxSizeBytes)
        {
            if (imageStream == null || imageStream.Length == 0)
                return Result.Fail(ErrorCode.InvalidStream);

            if (imageStream.Length > maxSizeBytes)
                return Result.Fail(ErrorCode.ImageTooLarge);

            try
            {
                if (imageStream.CanSeek)
                    imageStream.Position = 0;

                var info = Image.Identify(imageStream);

                if (info == null)
                    return Result.Fail(ErrorCode.CorruptedImage);

                if (info.Width > maxWidth || info.Height > maxHeight)
                    return Result.Fail(ErrorCode.InvalidImage);

                return Result.Ok(Operation.Validated);
            }
            catch
            {
                return Result.Fail(ErrorCode.CorruptedImage);
            }
        }


    }
}
