using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using WebpConverter.Common.Options;
using WebpConverter.Feature.DeleteImage;
using WebpConverter.Feature.ProcessImage;
using WebpConverter.Infrastructure;
using WebpConverter.Infrastructure.Compression;
using WebpConverter.Infrastructure.FileSystem;
using WebpConverter.Infrastructure.Logging;

Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);


var logOptions = builder.Configuration
    .GetSection(LogOptions.SectionName)
    .Get<LogOptions>() ?? new();

logOptions.Validate();
logOptions.EnsureDirectoryExists();

const string OutputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}";

var isDevelopment = builder.Environment.IsDevelopment();

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Is(logOptions.MinimumLevel)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(
        path: logOptions.GetFilePath(),
        outputTemplate: OutputTemplate,
        rollingInterval: RollingInterval.Infinite,
        retainedFileCountLimit: 1,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1));

if (isDevelopment)
{
    loggerConfiguration = loggerConfiguration
        .MinimumLevel.Verbose()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .WriteTo.Console(outputTemplate: OutputTemplate);
}

Log.Logger = loggerConfiguration.CreateLogger();
builder.Host.UseSerilog();

var keysDirectory = Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys");
Directory.CreateDirectory(keysDirectory);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysDirectory))
    .SetApplicationName("MediaProccesing");

// Configure Forwarded Headers for Nginx
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddOptions<AppOptions>()
      .BindConfiguration("AppOptions")
      .ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<AppOptions>, AppOptionsValidator>();


builder.Services.AddControllers();
builder.Services.AddProblemDetails(options =>
{

    options.CustomizeProblemDetails = ctx =>
    {
        var problem = ctx.ProblemDetails;

        problem.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
        problem.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
        problem.Extensions["timestamp"] = DateTime.UtcNow.ToString("o");  // ISO 8601


        if (!ctx.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            problem.Detail = null;
        }
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped<DeleteImageHandler>();
builder.Services.AddScoped<ProcessImageHandler>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<DirectoryService>();
builder.Services.AddScoped<WebPCompressor>();

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "MediaProcessing v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.MapControllers();

app.Run();