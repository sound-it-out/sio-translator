using Microsoft.Extensions.DependencyInjection;
using SIO.Domain.Translations;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Local.Files;
using SIO.Infrastructure.Local.Translations;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.Local.Extensions
{
    public static class ServiceCollectionExtensions
    {
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
