using MediaProcessing.Common.Enums;
using MediaProcessing.Common.Results;
using SixLabors.ImageSharp;

namespace MediaProcessing.Feature.ProcessImage;
// TODO: [Removal/Validation] Remove all logic related to image dimension validation.
// The parameters and checks for `maxWidth` and `maxHeight` should be fully deleted 
// from the class/method signature and implementation.
/* 
 * Reverting previous requirements change regarding image dimensions. 
 * The classes, properties (`int maxWidth`, `int maxHeight`), and all associated 
 * validation checks related to maximum width and height are now deemed unnecessary 
 * and must be removed to streamline the code base.
 */
/**
 * Architectural Refactoring: Delegation of Responsibility.
 * 
 * Action: Move all business rules, configuration reads (`maxFileSize`, `quality`),
 * and complex validation logic out of this controller method.
 * 
 * Why: To adhere to Separation of Concerns (SoC). The Controller should only handle 
 * HTTP requests/responses (API boundaries). All domain-specific validation must live 
 * in a dedicated service layer (e.g., `IImageCompressionService`). This keeps the controller clean, focused on flow control, and highly testable.
 */
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

            var info = Image.Identify(imageStream);

            if (info == null)
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