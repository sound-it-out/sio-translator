using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Google.Cloud.TextToSpeech.V1;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Google.Translations;
using SIO.Infrastructure.Translations;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.Google.Tests.Translations.GoogleSpeechSynthesizer.TranslateTextAsync
{
    public class WhenMaximumCharactersIsExceeded : GoogleSpeechSynthesizerSpecification
    {
        private const string _fiveHundredCharacters = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed viverra justo sed elit eleifend, non sagittis dolor consequat. Duis erat odio, sodales non bibendum vitae, elementum at augue. Phasellus tristique semper nisl ac mattis. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed viverra justo sed elit eleifend, non sagittis dolor consequat. Duis erat odio, sodales non bibendum vitae, elementum at augue. Phasellus tristique semper nisl ac mattis. Phasellus tristique semper nisl ac ma.";
        private GoogleSpeechRequest _request;
        private int _chunksToBeExectued;
        private int _chunksExectued;

        public WhenMaximumCharactersIsExceeded(ConfigurationFixture configurationFixture, SpeechSynthesizerFixture speechSynthesizerFixture) : base(configurationFixture, speechSynthesizerFixture)
        {
        }

        protected override async Task<ISpeechResult> Given()
        {
            return await SpeechSynthesizer.TranslateTextAsync(_request);
        }

        protected override Task When()
        {
            ExceptionMode = Testing.Abstractions.ExceptionMode.Record;
            var sb = new StringBuilder();

            for (int i = 0; i < 350; i++)
            {
                sb.Append(_fiveHundredCharacters);
            }

            var textChunks = sb.ToString().ChunkWithDelimeters(5000, '.', '!', '?', ')', '"', '}', ']');
            _chunksToBeExectued = textChunks.Count();

            _request = new GoogleSpeechRequest(
                voiceSelection: new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    SsmlGender = SsmlVoiceGender.Neutral
                },
                audioConfig: new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                },
                content: textChunks,
                callback: length =>
                {
                    Interlocked.Increment(ref _chunksExectued);
                }
            );

            return Task.CompletedTask;
        }

        [Integration]
        public void NoExceptionsShouldBeThrown()
        {
            Exception.Should().BeNull();
        }

        [Integration]
        public void AllChunksShouldBeExecuted()
        {
            _chunksExectued.Should().Be(_chunksToBeExectued);
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
