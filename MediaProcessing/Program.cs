using Microsoft.AspNetCore.DataProtection;
using Serilog;



Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();
        
var builder = WebApplication.CreateBuilder(args);

var keysDirectory = Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys");
if (!Directory.Exists(keysDirectory))
{
    Directory.CreateDirectory(keysDirectory);
}
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysDirectory))
    .SetApplicationName("MediaProccesing");

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {        
        options.SwaggerEndpoint("/openapi/v1.json", "MediaProccesor v1");     
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

/*app.MapPost("/process-image", async (
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
.DisableAntiforgery();*/

app.Run();

