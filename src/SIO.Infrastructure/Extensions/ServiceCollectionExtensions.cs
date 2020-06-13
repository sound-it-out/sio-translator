using Microsoft.Extensions.DependencyInjection;
using SIO.Domain.Translations;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Files.Local;
using SIO.Infrastructure.Translations;
using SIO.Infrastructure.Translations.Local;

namespace SIO.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection source)
        {
            source.AddHostedService<EventConsumer>();
            source.AddTransient<IEventPublisher, EventPublisher>();
            return source;
        }
        public static IServiceCollection AddLocalFiles(this IServiceCollection source)
        {
            source.AddTransient<IFileClient, LocalFileClient>();
            return source;
        }

        public static IServiceCollection AddLocalTranslations(this IServiceCollection source)
        {
            source.AddHostedService<BackgroundTranslator<LocalTranslation>>();
            source.AddSingleton<LocalTranslation>();
            source.AddTransient<ITranslationWorker<LocalTranslation>, LocalTranslationWorker>();
            return source;
        }
    }
}
