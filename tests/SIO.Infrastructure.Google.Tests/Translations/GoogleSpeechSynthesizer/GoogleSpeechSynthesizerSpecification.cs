using System;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.Google.Extensions;
using SIO.Infrastructure.Google.Translations;
using SIO.Infrastructure.Translations;
using SIO.Testing.Abstractions;
using Xunit;

namespace SIO.Infrastructure.Google.Tests.Translations.GoogleSpeechSynthesizer
{
    public abstract class GoogleSpeechSynthesizerSpecification : SpecificationWithConfiguration<ConfigurationFixture, ISpeechResult>, IClassFixture<SpeechSynthesizerFixture>
    {
        private readonly Lazy<SpeechSynthesizerFixture> _speechSynthesizerFixture;
        protected ISpeechSynthesizer<GoogleSpeechRequest> SpeechSynthesizer => _speechSynthesizerFixture.Value;

        public GoogleSpeechSynthesizerSpecification(ConfigurationFixture configurationFixture, SpeechSynthesizerFixture speechSynthesizerFixture) : base(configurationFixture)
        {
            _speechSynthesizerFixture = new Lazy<SpeechSynthesizerFixture>(() => 
            {
                speechSynthesizerFixture.InitSynthesizer(_serviceProvider.GetRequiredService<ISpeechSynthesizer<GoogleSpeechRequest>>());
                return speechSynthesizerFixture;
            });
        }

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);
            services.AddGoogleConfiguration(_configurationFixture.Configuration)
                .AddGoogleSpeechSynthesizer();
        }
    }
}
