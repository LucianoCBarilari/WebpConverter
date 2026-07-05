using Microsoft.AspNetCore.Mvc;
using WebpConverter.Common.Enums;

namespace WebpConverter.Feature.ProcessImage;

public class CompressImageHttpRequest
{
    [FromForm]
    public IFormFile File { get; set; } = default!;

    [FromForm]
    public string? FileName { get; set; }
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
        var command = new ImageToProcess(stream, request.FileName);

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
                detail: result.Message ?? "La imagen supera el tamaño permitido."),
            ErrorCode.InvalidImage => Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Image",
                detail: result.Message ?? "La imagen no es válida."),
            ErrorCode.CorruptedImage => Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Corrupted Image",
                detail: result.Message ?? "La imagen está dañada."),
            ErrorCode.Unauthorized => Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden",
                detail: result.Message ?? "No tienes permisos para guardar la imagen."),
            ErrorCode.IOError => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "I/O Error",
                detail: result.Message ?? "Error de escritura en disco."),
            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unexpected Error",
                detail: result.Message ?? "Ocurrió un error inesperado.")
        };
    }
}