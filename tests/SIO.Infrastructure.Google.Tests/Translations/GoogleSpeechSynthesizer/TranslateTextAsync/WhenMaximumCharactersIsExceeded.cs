using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clipboard;
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

        public WhenMaximumCharactersIsExceeded(ConfigurationFixture configurationFixture, SpeechSynthesizerFixture speechSynthesizerFixture) : base(configurationFixture, speechSynthesizerFixture)
        {
        }

        protected override async Task<ISpeechResult> Given()
        {
            return await SpeechSynthesizer.TranslateTextAsync(_request);
        }

        protected override async Task When()
        {
            ExceptionMode = Testing.Abstractions.ExceptionMode.Record;
            var sb = new StringBuilder();

            for (int i = 0; i < 350; i++)
            {
                sb.Append(_fiveHundredCharacters);
            }
            var text = "";

            using (var textExtractor = TextExtractor.Open(@"C:\Users\matth\Desktop\test.txt"))
            {
                text = await textExtractor.ExtractAsync();
            }

            var textChunks = text.ChunkWithDelimeters(4800, '.', '!', '?', ')', '"', '}', ']');

            _request = new GoogleSpeechRequest(
                voiceSelection: new VoiceSelectionParams
                {
                    Name = ""
                },
                audioConfig: new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                },
                content: textChunks
            );
        }

        [Integration]
        public void NoExceptionsShouldBeThrown()
        {
            Exception.Should().BeNull();
        }

        [Integration]
        public async Task StreamShouldNotBeNullOrEmpty()
        {
            using (var stream = await Result.OpenStreamAsync())
            {
                stream.Should().NotBeNull();
                stream.Length.Should().BeGreaterThan(0);
                using (FileStream fs = new FileStream(@"C:\Users\matth\Desktop\test.mp3", FileMode.Create))
                {
                    await stream.CopyToAsync(fs);
                    fs.Flush();
                }
            }
        }
    }
}
