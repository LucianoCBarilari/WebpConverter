using MediaProcessorLibrary.Application.Interfaces;
using MediaProcessorLibrary.Application.UseCases;
using MediaProcessorLibrary.Common.Helpers;
using MediaProcessorLibrary.Infrastructure.FileSystem;
using MediaProcessorLibrary.Infrastructure.ImageConversion;
using MediaProcessorLibrary.Infrastructure.Imaging;
using Microsoft.Extensions.DependencyInjection;

namespace MediaProcessorLibrary.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediaProcessor(
       this IServiceCollection services)
        {
            services.AddScoped<IImageProcessingService, ImageProcessingService>();

            services.AddScoped<IDirectoryService, DirectoryService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IImageValidator, ImageValidator>();
            services.AddScoped<IFormatCompress, FormatCompress>();
            services.AddScoped<IHelpers, Helper>();

            return services;
        }
    }
}
