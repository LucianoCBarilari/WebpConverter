namespace MediaProcessorLibrary;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediaProcessor(
   this IServiceCollection services)
    {
        services.AddScoped<IImageProcessingService,ImageProcessingService>();
        services.AddScoped<FileService>();
        services.AddScoped<DirectoryService>();
        services.AddScoped<ImageValidator>();
        services.AddScoped<WebPCompressor>();
        services.AddScoped<Utils>();

        return services;
    }
}
