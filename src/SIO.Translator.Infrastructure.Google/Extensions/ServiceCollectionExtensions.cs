using Microsoft.Extensions.DependencyInjection;
using SIO.Translator.Domain.Translations;
using SIO.Translator.Infrastructure.Google.Translations;
using SIO.Translator.Infrastructure.Translations;

namespace SIO.Translator.Infrastructure.Google.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGoogleTranslations(this IServiceCollection source)
        {
            source.AddHostedService<BackgroundTranslator<GoogleTranslation>>();
            source.AddSingleton<GoogleTranslation>();
            source.AddTransient<ISpeechSynthesizer<GoogleSpeechRequest>, GoogleSpeechSynthesizer>();
            source.AddTransient<ITranslationWorker<GoogleTranslation>, GoogleTranslationWorker>();

            return source;
        }
    }
}
