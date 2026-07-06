using Microsoft.AspNetCore.Mvc;
using WebpConverter.Common.Enums;

namespace WebpConverter.Feature.ProcessImage;

public class CompressImageHttpRequest
{
    [FromForm]
    public IFormFile File { get; set; } = default!;

    [FromForm]
    public string? FileName { get; set; }

    [FromForm]
    public string? Subfolder { get; set; }
}

[ApiController]
[Route("/api/image-processor")]
[Consumes("multipart/form-data")]
public class ImageProcessorController(ProcessImageHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CompressImageAsync([FromForm] CompressImageHttpRequest request)
    {
        if (request?.File == null)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation Error",
                detail: "File is required.");
        }

        using var stream = request.File.OpenReadStream();
        var command = new ImageToProcess(stream, request.FileName, Subfolder: request.Subfolder);

        var result = await handler.HandleAsync(command);

        if (result.IsSuccess && !string.IsNullOrWhiteSpace(result.Value))
        {
            return Ok(result.Value);
        }

        var error = result.Error ?? ErrorCode.Unexpected;

        return error switch
        {
            ErrorCode.ImageTooLarge => Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Image Too Large",
                detail: result.Message ?? "The image exceeds the allowed maximum size."),
            ErrorCode.InvalidImage => Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Image",
                detail: result.Message ?? "The image is invalid."),
            ErrorCode.CorruptedImage => Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Corrupted Image",
                detail: result.Message ?? "The image is corrupted."),
            ErrorCode.Unauthorized => Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden",
                detail: result.Message ?? "You do not have permission to save the image."),
            ErrorCode.IOError => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "I/O Error",
                detail: result.Message ?? "Disk I/O error."),
            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unexpected Error",
                detail: result.Message ?? "An unexpected error occurred.")
        };
    }
}