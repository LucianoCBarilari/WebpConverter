using SixLabors.ImageSharp;

namespace MediaProcessorLibrary.Feature.Tools;
public class ImageValidator
{
    /// <summary>
    /// Validates an image stream against size and dimension constraints using the Result Pattern.
    /// </summary>
    public ResultMedia Validate(
        Stream imageStream,
        int maxWidth,
        int maxHeight,
        long maxSizeBytes)
    {
        if (imageStream == null || imageStream.Length == 0)
            return ResultMedia.Fail(ErrorCode.InvalidStream);

        if (imageStream.Length > maxSizeBytes)
            return ResultMedia.Fail(ErrorCode.ImageTooLarge);

        try
        {
            if (imageStream.CanSeek)
                imageStream.Position = 0;

            var info = Image.Identify(imageStream);

            if (info == null)
                return ResultMedia.Fail(ErrorCode.CorruptedImage);

            if (info.Width > maxWidth || info.Height > maxHeight)
                return ResultMedia.Fail(ErrorCode.InvalidDimensions);

            return ResultMedia.Ok(Operation.Validated);
        }
        catch
        {
            return ResultMedia.Fail(ErrorCode.CorruptedImage);
        }
    }
}