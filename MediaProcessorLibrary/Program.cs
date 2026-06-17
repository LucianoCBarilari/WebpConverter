using MediaProcessorLibrary;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddMediaProcessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/process-image", async (
    IFormFile file,
    [FromForm] int maxWidth,
    [FromForm] int maxHeight,
    [FromForm] long maxSizeBytes,
    [FromForm] string outputDirectory,
    [FromForm] string outputFileName,
    [FromForm] int quality,
    IImageProcessingService processingService) =>
{
    using var stream = file.OpenReadStream();
    var request = new ImageProcessingRequest
    {
        ImageStream = stream,
        MaxWidth = maxWidth,
        MaxHeight = maxHeight,
        MaxSizeBytes = maxSizeBytes,
        OutputDirectory = outputDirectory,
        OutputFileName = outputFileName,
        Quality = quality
    };

    var result = await processingService.ImageProcessAsync(request);

    return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
})
.WithName("ProcessImage")
.WithOpenApi()
.DisableAntiforgery();

app.Run();

