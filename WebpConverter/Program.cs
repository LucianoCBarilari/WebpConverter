using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using WebpConverter.Common.Options;
using WebpConverter;
using WebpConverter.Feature.DeleteImage;
using WebpConverter.Feature.ProcessImage;
using WebpConverter.Infrastructure;
using WebpConverter.Infrastructure.Compression;
using WebpConverter.Infrastructure.FileSystem;
using WebpConverter.Infrastructure.Interceptors;
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


builder.Services.AddOptions<AppOptions>()
      .BindConfiguration("AppOptions")
      .ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<AppOptions>, AppOptionsValidator>();


builder.Services.AddScoped<IDeleteImageHandler, DeleteImageHandler>();
builder.Services.AddScoped<ProcessImageHandler>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<DirectoryService>();
builder.Services.AddScoped<IWebPCompressor, WebPCompressor>();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionInterceptor>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.MapGrpcService<ImageProcessorService>();
app.Run();

public partial class Program { }