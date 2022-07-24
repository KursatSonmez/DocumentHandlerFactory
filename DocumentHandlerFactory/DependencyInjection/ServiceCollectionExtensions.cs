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
            DocumentHandlerSettings _settings = DocumentHandlerSettings.DefaultValue();

            settings?.Invoke(_settings);

            _settings.Validate();

            services.AddSingleton(x => _settings);

            return services
                .AddScoped<IDocumentHandlerService, DocumentHandlerService>()
                .AddScoped<ITempFileHandlerService, TempFileHandlerService>();
        }
    }
}
