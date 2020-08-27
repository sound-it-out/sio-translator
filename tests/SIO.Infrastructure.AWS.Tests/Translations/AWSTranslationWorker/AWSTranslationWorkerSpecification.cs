using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Infrastructure.AWS.Extensions;
using SIO.Infrastructure.AWS.Translations;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Translations;
using SIO.Testing.Abstractions;
using SIO.Testing.Extensions;

namespace SIO.Infrastructure.AWS.Tests.Translations.AWSTranslationWorker
{
    public abstract class AWSTranslationWorkerSpecification : Specification
    {
        protected ITranslationWorker<AWS.Translations.AWSTranslation> TranslationWorker => _serviceProvider.GetRequiredService<ITranslationWorker<AWS.Translations.AWSTranslation>>();

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);

            services.AddOpenEventSourcing()
                .AddEvents()
                .AddJsonSerializers();

            services.AddInfrastructure()
                .AddInMemoryDatabase()
                .AddInMemoryFiles()
                .AddInMemoryEventBus()
                .AddAWSTranslationWorker()
                .AddLogging(logging => logging.AddConsole());
        }
    }
}
