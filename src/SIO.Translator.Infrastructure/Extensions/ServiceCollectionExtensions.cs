using Microsoft.Extensions.DependencyInjection;
using SIO.Translator.Domain.Translations;
using SIO.Translator.Infrastructure.Events;
using SIO.Translator.Infrastructure.Files;
using SIO.Translator.Infrastructure.Files.Local;
using SIO.Translator.Infrastructure.Translations;
using SIO.Translator.Infrastructure.Translations.Local;

namespace SIO.Translator.Infrastructure.Extensions
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
