using Microsoft.Extensions.DependencyInjection;
using SIO.Translator.Domain.Translations;
using SIO.Translator.Infrastructure.AWS.Files;
using SIO.Translator.Infrastructure.AWS.Translations;
using SIO.Translator.Infrastructure.Files;
using SIO.Translator.Infrastructure.Translations;

namespace SIO.Translator.Infrastructure.AWS.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAWSTranslations(this IServiceCollection source)
        {
            source.AddHostedService<BackgroundTranslator<AWSTranslation>>();
            source.AddSingleton<AWSTranslation>();
            source.AddTransient<ISpeechSynthesizer<AWSSpeechRequest>, AWSSpeechSynthesizer>();
            source.AddTransient<ITranslationWorker<AWSTranslation>, AWSTranslationWorker>();

            return source;
        }

        public static IServiceCollection AddAWSFiles(this IServiceCollection source)
        {
            source.AddTransient<IFileClient, AWSFileClient>();

            return source;
        }
    }
}
