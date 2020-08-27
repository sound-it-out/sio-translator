using System;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using SIO.Domain.Translations;
using SIO.Infrastructure.AWS.Extensions;
using SIO.Infrastructure.AWS.Tests.Stubs;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Translations;
using SIO.Testing.Abstractions;
using SIO.Testing.Extensions;
using Xunit;

namespace SIO.Infrastructure.AWS.Tests.Translations.AWSTranslation
{
    public abstract class AWSTranslationSpecification : Specification, IClassFixture<AWSTranslationFixture>, IClassFixture<DocumentUploadedFixture>
    {
        private Lazy<AWSTranslationFixture> _awsTranslationFixture;
        protected Translation Translation => _awsTranslationFixture.Value;
        protected readonly DocumentUploadedFixture _eventFixture;

        public AWSTranslationSpecification(AWSTranslationFixture awsTranslationFixture, DocumentUploadedFixture eventFixture, TranslationType translationType)
        {
            _awsTranslationFixture = new Lazy<AWSTranslationFixture>(() =>
            {
                awsTranslationFixture.InitSynthesizer(_serviceProvider.GetRequiredService<AWS.Translations.AWSTranslation>());
                return awsTranslationFixture;
            });

            eventFixture.Init(translationType);
            _eventFixture = eventFixture;
        }

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);
            services.AddSingleton<ITranslationWorker<AWS.Translations.AWSTranslation>, InMemoryAWSTranslationWorker>();

            services.AddOpenEventSourcing()
                .AddEvents()
                .AddJsonSerializers();

            services.AddInfrastructure()
                .AddInMemoryDatabase()
                .AddInMemoryFiles()
                .AddInMemoryEventBus()
                .AddAWSTranslation()
                .AddLogging(logging => logging.AddConsole());

            GlobalConfiguration.Configuration.UseMemoryStorage();
        }
    }
}
