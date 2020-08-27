using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIO.Domain.Translations;
using SIO.Infrastructure.Google.Translations;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.Google.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGoogleInfrastructure(this IServiceCollection source)
        {
            source.AddHostedService<BackgroundTranslator<GoogleTranslation>>();
            source.AddGoogleTranslation();
            source.AddGoogleSpeechSynthesizer();
            source.AddGoogleTranslationWorker();

            return source;
        }

        public static IServiceCollection AddGoogleConfiguration(this IServiceCollection source, IConfiguration configuration)
        {
            source.Configure<GoogleCredentialOptions>(configuration.GetSection("Google:Credentails"));

            return source;
        }

        public static IServiceCollection AddGoogleTranslation(this IServiceCollection source)
        {
            source.AddScoped<GoogleTranslation>();
            return source;
        }

        public static IServiceCollection AddGoogleTranslationWorker(this IServiceCollection source)
        {
            source.AddScoped<ITranslationWorker<GoogleTranslation>, GoogleTranslationWorker>();
            return source;
        }

        public static IServiceCollection AddGoogleSpeechSynthesizer(this IServiceCollection source)
        {
            source.AddScoped<ISpeechSynthesizer<GoogleSpeechRequest>, GoogleSpeechSynthesizer>();
            return source;
        }
    }
}
