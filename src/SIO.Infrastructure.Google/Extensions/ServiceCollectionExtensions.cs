using Microsoft.Extensions.DependencyInjection;
using SIO.Domain.Translations;
using SIO.Infrastructure.Google.Translations;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.Google.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGoogleTranslations(this IServiceCollection source)
        {
            source.AddHostedService<BackgroundTranslator<GoogleTranslation>>();
            source.AddScoped<GoogleTranslation>();
            source.AddScoped<ISpeechSynthesizer<GoogleSpeechRequest>, GoogleSpeechSynthesizer>();
            source.AddScoped<ITranslationWorker<GoogleTranslation>, GoogleTranslationWorker>();

            return source;
        }
    }
}
