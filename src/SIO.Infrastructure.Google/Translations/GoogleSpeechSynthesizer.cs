using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Translations;
using System.Linq;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Google.Translations
{
    internal sealed class GoogleSpeechSynthesizer : ISpeechSynthesizer<GoogleSpeechRequest>
    {
        private readonly TextToSpeechClient _client;

        public GoogleSpeechSynthesizer()
        {
            var credentials = GoogleCredential.FromFile(@"C:\Users\matth\Downloads\My First Project-06fc82e47656.json");

            var builder = new TextToSpeechClientBuilder();
            builder.ChannelCredentials = credentials.ToChannelCredentials();
            _client = builder.Build();
        }

        public async ValueTask<ISpeechResult> TranslateTextAsync(GoogleSpeechRequest request)
        {
            var result = new GoogleSpeechResult();

            foreach (var requestChunks in request.Content.Chunk(300))
            {
                await Task.WhenAll(requestChunks.Select((c, i) =>
                    QueueText(c, i, request, result)
                ));

                // Need to wait some time due to rate limits
                await Task.Delay(1);
            }

            return result;
        }

        private async Task QueueText(string text, int index, GoogleSpeechRequest request, GoogleSpeechResult result)
        {
            var response = await _client.SynthesizeSpeechAsync(
                input: new SynthesisInput
                {
                    Text = text
                },
                voice: request.VoiceSelection,
                audioConfig: request.AudioConfig
            );

            result.DigestBytes(index, response.AudioContent);
            request.CallBack(text.Length);
        }
    }
}
