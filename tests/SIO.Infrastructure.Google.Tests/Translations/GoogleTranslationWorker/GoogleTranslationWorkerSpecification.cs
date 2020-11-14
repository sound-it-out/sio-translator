using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Google.Extensions;
using SIO.Infrastructure.Translations;
using SIO.Testing.Abstractions;
using SIO.Testing.Extensions;

namespace SIO.Infrastructure.Google.Tests.Translations.GoogleTranslationWorker
{
    public abstract class GoogleTranslationWorkerSpecification : Specification
    {
        protected ITranslationWorker<Google.Translations.GoogleTranslation> TranslationWorker => _serviceProvider.GetRequiredService<ITranslationWorker<Google.Translations.GoogleTranslation>>();

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
                .AddGoogleTranslationWorker()
                .AddLogging(logging => logging.AddConsole());
        }
    }
}
