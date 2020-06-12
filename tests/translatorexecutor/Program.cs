using Clipboard;
using Google.Cloud.TextToSpeech.V1;
using Microsoft.Extensions.DependencyInjection;
using SIO.Translator.Infrastructure.Extensions;
using SIO.Translator.Infrastructure.Google.Extensions;
using SIO.Translator.Infrastructure.Google.Translations;
using SIO.Translator.Infrastructure.Translations;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace translatorexecutor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGoogleTranslations();
            serviceCollection.AddLocalFiles();

            var provider = serviceCollection.BuildServiceProvider();
            var translator = provider.GetRequiredService<ISpeechSynthesizer<GoogleSpeechRequest>>();
            var result = await translator.TranslateTextAsync(new GoogleSpeechRequest(
                    voiceSelection: new VoiceSelectionParams
                    {
                        LanguageCode = "en-US",
                        SsmlGender = SsmlVoiceGender.Male,
                        Name = "en-GB-Wavenet-A"
                    },
                    audioConfig: new AudioConfig
                    {
                        AudioEncoding = AudioEncoding.Mp3
                    },
                    content: TextExtractor.Open(@"E:\OneDrive\Work\R&D\Marketing material\Addicted to Power BI Us too. 6 reasons why we just can't stop.docx").Extract().ChunkWithDelimeters(5000, '.', '!', '?', ')', '"', '}', ']'),
                    callback: async length =>
                    {
                        var test = 1;
                    }
                ));

            using (var s = File.OpenWrite(@"E:\test.mp3"))
            {
                var stream = await result.OpenStreamAsync();
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(s);
            }
        }
    }
}
