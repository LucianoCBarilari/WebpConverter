using Grpc.Core;

namespace WebpConverter.Feature.ProcessImage;

public class ImageProcessorService(
    ProcessImageHandler processImageHandler, 
    ILogger<ImageProcessorService> logger) : ImageProcessor.ImageProcessorBase
{
    public override async Task<ConvertImageReply> ConvertImage(ConvertImageRequest request, ServerCallContext context)
    {
        logger.LogInformation("Received request to convert image {FileName}", request.FileName);

        // Map the gRPC request to the existing domain model
        var imageToProcess = new ImageToProcess(
            new MemoryStream(request.ImageData.ToByteArray()),
            request.FileName,
            string.IsNullOrEmpty(request.PreviousFileName) ? null : request.PreviousFileName,
            string.IsNullOrEmpty(request.Subfolder) ? null : request.Subfolder
        );

        // Reuse existing business logic intact
        var result = await processImageHandler.HandleAsync(imageToProcess, context.CancellationToken);

        // Map the domain result to the gRPC reply
        if (result.IsSuccess)
        {
            return new ConvertImageReply
            {
                Success = true,
                PublicPath = result.Value
            };
        }

        // If it failed due to validation or business rules
        return new ConvertImageReply
        {
            Success = false,
            ErrorCode = result.Error.ToString(),
            ErrorMessage = result.Message
        };
    }
}
