using System;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.AWS.Extensions;
using SIO.Infrastructure.AWS.Translations;
using SIO.Infrastructure.Translations;
using SIO.Testing.Abstractions;
using SIO.Testing.Extensions;
using Xunit;

namespace SIO.Infrastructure.AWS.Tests.Translations.AWSSpeechSynthesizer
{
    public abstract class AWSSpeechSynthesizerSpecification : SpecificationWithConfiguration<ConfigurationFixture, ISpeechResult>, IClassFixture<SpeechSynthesizerFixture>
    {
        private readonly Lazy<SpeechSynthesizerFixture> _speechSynthesizerFixture;
        protected ISpeechSynthesizer<AWSSpeechRequest> SpeechSynthesizer => _speechSynthesizerFixture.Value;

        public AWSSpeechSynthesizerSpecification(ConfigurationFixture configurationFixture, SpeechSynthesizerFixture speechSynthesizerFixture) : base(configurationFixture)
        {
            _speechSynthesizerFixture = new Lazy<SpeechSynthesizerFixture>(() => 
            {
                speechSynthesizerFixture.InitSynthesizer(_serviceProvider.GetRequiredService<ISpeechSynthesizer<AWSSpeechRequest>>());
                return speechSynthesizerFixture;
            });
        }

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);
            services.AddAWSConfiguration(_configurationFixture.Configuration)
                .AddAWSSpeechSynthesizer()
                .AddAWSFiles();
        }
    }
}
