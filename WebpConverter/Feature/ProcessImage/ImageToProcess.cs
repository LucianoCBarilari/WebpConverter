namespace WebpConverter.Feature.ProcessImage;

/// <summary>
/// Represents the unified command/DTO for the image processing vertical slice.
/// </summary>
public record ImageToProcess(
    Stream ImageStream,
    string FileName,
    string? PreviousFileName = null,
    string? Subfolder = null
);
