using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIO.Domain.Translations;
using SIO.Infrastructure.AWS.Files;
using SIO.Infrastructure.AWS.Translations;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.AWS.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAWSInfrastructure(this IServiceCollection source)
        {
            source.AddHostedService<BackgroundTranslator<AWSTranslation>>();
            source.AddAWSTranslation();
            source.AddAWSSpeechSynthesizer();
            source.AddAWSTranslationWorker();

            return source;
        }

        public static IServiceCollection AddAWSConfiguration(this IServiceCollection source, IConfiguration configuration)
        {
            source.Configure<AWSCredentialOptions>(configuration.GetSection("AWS:Credentails"));
            source.Configure<AWSFileOptions>(configuration.GetSection("AWS:S3"));
            source.Configure<AWSTranslationOptions>(configuration.GetSection("AWS:Polly"));
            return source;
        }

        public static IServiceCollection AddAWSTranslation(this IServiceCollection source)
        {
            source.AddScoped<AWSTranslation>();
            return source;
        }

        public static IServiceCollection AddAWSTranslationWorker(this IServiceCollection source)
        {
            source.AddScoped<ITranslationWorker<AWSTranslation>, AWSTranslationWorker>();
            return source;
        }

        public static IServiceCollection AddAWSSpeechSynthesizer(this IServiceCollection source)
        {
            source.AddScoped<ISpeechSynthesizer<AWSSpeechRequest>, AWSSpeechSynthesizer>();
            return source;
        }

        public static IServiceCollection AddAWSFiles(this IServiceCollection source)
        {
            source.AddTransient<IFileClient, AWSFileClient>();

            return source;
        }
    }
}
