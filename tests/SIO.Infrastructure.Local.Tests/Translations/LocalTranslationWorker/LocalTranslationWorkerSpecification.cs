using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Local.Extensions;
using SIO.Infrastructure.Translations;
using SIO.Testing.Abstractions;
using SIO.Testing.Extensions;

namespace SIO.Infrastructure.Local.Tests.Translations.LocalTranslationWorker
{
    public abstract class LocalTranslationWorkerSpecification : Specification
    {
        protected ITranslationWorker<Local.Translations.LocalTranslation> TranslationWorker => _serviceProvider.GetRequiredService<ITranslationWorker<Local.Translations.LocalTranslation>>();

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
                .AddLocalTranslationWorker()
                .AddLogging(logging => logging.AddConsole());
        }
    }
}
