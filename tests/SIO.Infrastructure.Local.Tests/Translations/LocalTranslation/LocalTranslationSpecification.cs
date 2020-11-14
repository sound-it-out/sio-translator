using System;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Domain.Translations;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Local.Extensions;
using SIO.Infrastructure.Local.Tests.Stubs;
using SIO.Infrastructure.Translations;
using SIO.Testing.Abstractions;
using SIO.Testing.Extensions;
using Xunit;

namespace SIO.Infrastructure.Local.Tests.Translations.LocalTranslation
{
    public abstract class LocalTranslationSpecification : Specification, IClassFixture<LocalTranslationFixture>, IClassFixture<DocumentUploadedFixture>
    {
        private Lazy<LocalTranslationFixture> _localTranslationFixture;
        protected Translation Translation => _localTranslationFixture.Value;
        protected readonly DocumentUploadedFixture _eventFixture;

        public LocalTranslationSpecification(LocalTranslationFixture localTranslationFixture, DocumentUploadedFixture eventFixture, TranslationType translationType)
        {
            _localTranslationFixture = new Lazy<LocalTranslationFixture>(() =>
            {
                localTranslationFixture.InitSynthesizer(_serviceProvider.GetRequiredService<Local.Translations.LocalTranslation>());
                return localTranslationFixture;
            });

            eventFixture.Init(translationType);
            _eventFixture = eventFixture;
        }

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);
            services.AddSingleton<ITranslationWorker<Local.Translations.LocalTranslation>, InMemoryLocalTranslationWorker>();

            services.AddOpenEventSourcing()
                .AddEvents()
                .AddJsonSerializers();

            services.AddInfrastructure()
                .AddInMemoryDatabase()
                .AddInMemoryFiles()
                .AddInMemoryEventBus()
                .AddLocalTranslation()
                .AddLogging(logging => logging.AddConsole());

            GlobalConfiguration.Configuration.UseMemoryStorage();
        }
    }
}
