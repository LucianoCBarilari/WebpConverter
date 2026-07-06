using SkiaSharp;
using WebpConverter.Common.Enums;
using WebpConverter.Common.Results;

namespace WebpConverter.Feature.ProcessImage;
public static class ImageValidator
{
    /// <summary>
    /// Validates only that the stream does not exceed the allowed size constraints.
    /// </summary>
    public static ResultMedia ValidateSize(Stream imageStream, long maxSizeBytes)
    {
        if (imageStream == null || imageStream.Length == 0)
            return ResultMedia.Fail(ErrorCode.InvalidStream);

        if (imageStream.Length > maxSizeBytes)
            return ResultMedia.Fail(ErrorCode.ImageTooLarge);

        return ResultMedia.Ok(Operation.Validated);
    }
    /// <summary>
    /// Validates only the file integrity and ensures it is a readable, uncorrupted image.
    /// </summary>
    public static ResultMedia ValidateImage(Stream imageStream)
    {
        if (imageStream == null || imageStream.Length == 0)
            return ResultMedia.Fail(ErrorCode.InvalidStream);

        try
        {
            long originalPosition = imageStream.CanSeek ? imageStream.Position : 0;

            if (imageStream.CanSeek)
                imageStream.Position = 0;
            
            using var skStream = new SKManagedStream(imageStream, disposeManagedStream: false);
            using var codec = SKCodec.Create(skStream);

            if (codec == null)
                return ResultMedia.Fail(ErrorCode.CorruptedImage);

            if (imageStream.CanSeek)
                imageStream.Position = originalPosition;

            return ResultMedia.Ok(Operation.Validated);
        }
        catch
        {
            return ResultMedia.Fail(ErrorCode.CorruptedImage);
        }
    }
}