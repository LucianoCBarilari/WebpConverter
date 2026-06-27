using MediaProcessing.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediaProcessing.Feature.ProcessImage;

[ApiController]
[Route("v2/api/image-processor")]
public class ImageProcessorController(IImageProcessingAppService imgProcessing) : ControllerBase
{
    [Authorize(Policy = "CanWrite")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ImageProcessRequest request)
    {
        if (request.File == null)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation Error",
                detail: "File is required.");
        }

        const int maxFileSize = 2 * 1024 * 1024;
        var fileName = string.IsNullOrWhiteSpace(request.FileName)
            ? Path.GetFileNameWithoutExtension(request.File.FileName)
            : request.FileName;
        var quality = 80;

        var result = await imgProcessing.ProcessImage(request.File, maxFileSize, fileName, request.ImageSizeId, quality);

        if (result.IsSuccess && !string.IsNullOrWhiteSpace(result.Value!))
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

