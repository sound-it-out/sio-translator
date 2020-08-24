using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.EntityFrameworkCore.Extensions;
using OpenEventSourcing.Extensions;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Google.Extensions;
using SIO.Infrastructure.Google.Tests.Stubs;
using SIO.Infrastructure.Google.Translations;
using SIO.Infrastructure.Translations;
using SIO.Testing.Abstractions;
using SIO.Testing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace SIO.Infrastructure.Google.Tests.Translations.GoogleTranslationWorker
{
    public abstract class GoogleTranslationWorkerSpecification : Specification
    {
        protected ITranslationWorker<GoogleTranslation> TranslationWorker => _serviceProvider.GetRequiredService<ITranslationWorker<GoogleTranslation>>();

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
