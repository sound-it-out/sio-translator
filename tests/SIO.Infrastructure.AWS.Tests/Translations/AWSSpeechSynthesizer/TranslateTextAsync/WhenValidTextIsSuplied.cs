using System.Threading;
using System.Threading.Tasks;
using Amazon.Polly;
using FluentAssertions;
using SIO.Infrastructure.AWS.Translations;
using SIO.Infrastructure.Translations;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.AWS.Tests.Translations.AWSSpeechSynthesizer.TranslateTextAsync
{
    public class WhenValidTextIsSuplied : AWSSpeechSynthesizerSpecification
    {
        private const string _fiveHundredCharacters = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed viverra justo sed elit eleifend, non sagittis dolor consequat. Duis erat odio, sodales non bibendum vitae, elementum at augue. Phasellus tristique semper nisl ac mattis. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed viverra justo sed elit eleifend, non sagittis dolor consequat. Duis erat odio, sodales non bibendum vitae, elementum at augue. Phasellus tristique semper nisl ac mattis. Phasellus tristique semper nisl ac ma.";
        private AWSSpeechRequest _request;

        public WhenValidTextIsSuplied(ConfigurationFixture configurationFixture, SpeechSynthesizerFixture speechSynthesizerFixture) : base(configurationFixture, speechSynthesizerFixture)
        {
        }

        protected override async Task<ISpeechResult> Given()
        {
            return await SpeechSynthesizer.TranslateTextAsync(_request);
        }

        protected override Task When()
        {
            _request = new AWSSpeechRequest
            {
                OutputFormat = OutputFormat.Mp3,
                Content = new string[] { _fiveHundredCharacters },
                VoiceId = VoiceId.Aditi
            };

            return Task.CompletedTask;
        }

        [Integration]
        public async Task StreamShouldNotBeNullOrEmpty()
        {
            using (var stream = await Result.OpenStreamAsync())
            {
                stream.Should().NotBeNull();
                stream.Length.Should().BeGreaterThan(0);
            }
        }
    }
}
