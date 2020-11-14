using System;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Domain.Translations;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Google.Extensions;
using SIO.Infrastructure.Google.Tests.Stubs;
using SIO.Infrastructure.Translations;
using SIO.Testing.Abstractions;
using SIO.Testing.Extensions;
using Xunit;

namespace SIO.Infrastructure.Google.Tests.Translations.GoogleTranslation
{
    public abstract class GoogleTranslationSpecification : Specification, IClassFixture<GoogleTranslationFixture>, IClassFixture<DocumentUploadedFixture>
    {
        private Lazy<GoogleTranslationFixture> _googleTranslationFixture;
        protected Translation Translation => _googleTranslationFixture.Value;
        protected readonly DocumentUploadedFixture _eventFixture;

        public GoogleTranslationSpecification(GoogleTranslationFixture googleTranslationFixture, DocumentUploadedFixture eventFixture, TranslationType translationType)
        {
            _googleTranslationFixture = new Lazy<GoogleTranslationFixture>(() =>
            {
                googleTranslationFixture.InitSynthesizer(_serviceProvider.GetRequiredService<Google.Translations.GoogleTranslation>());
                return googleTranslationFixture;
            });

            eventFixture.Init(translationType);
            _eventFixture = eventFixture;
        }

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);
            services.AddSingleton<ITranslationWorker<Google.Translations.GoogleTranslation>, InMemoryGoogleTranslationWorker>();

            services.AddOpenEventSourcing()
                .AddEvents()
                .AddJsonSerializers();

            services.AddInfrastructure()
                .AddInMemoryDatabase()
                .AddInMemoryFiles()
                .AddInMemoryEventBus()
                .AddGoogleTranslation()
                .AddLogging(logging => logging.AddConsole());

            GlobalConfiguration.Configuration.UseMemoryStorage();
        }
    }
}
