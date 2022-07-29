using DocumentHandlerFactory.Services;
using DocumentHandlerFactory.Services.Interfaces;
using DocumentHandlerFactory.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DocumentHandlerFactory.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDocumentHandlerFactory(this IServiceCollection services, Action<DocumentHandlerSettings> settings = null)
        {
            DocumentHandlerSettings documentHandlerSettings = DocumentHandlerSettings.DefaultValue();

            settings?.Invoke(documentHandlerSettings);

            documentHandlerSettings.Validate();

            return services
            .AddSingleton(documentHandlerSettings)
            .AddScoped<IDocumentHandlerService, DocumentHandlerService>();
        }

        public static IServiceCollection AddTempFileHandler(this IServiceCollection services, Action<TempFileSettings> settings = null)
        {
            TempFileSettings tempFileSettings = TempFileSettings.DefaultValue();

            settings?.Invoke(tempFileSettings);

            tempFileSettings.Validate();

            return services
            .AddSingleton(tempFileSettings)
            .AddScoped<ITempFileHandlerService, TempFileHandlerService>();
        }
    }
}