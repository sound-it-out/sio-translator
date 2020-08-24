﻿using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Translations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Google.Translations
{
    internal sealed class GoogleSpeechSynthesizer : ISpeechSynthesizer<GoogleSpeechRequest>
    {
        private readonly TextToSpeechClient _client;

        public GoogleSpeechSynthesizer(IOptions<GoogleCredentialOptions> googleCredentialOptions)
        {
            if (googleCredentialOptions == null)
                throw new ArgumentNullException(nameof(googleCredentialOptions));

            var credentials = GoogleCredential.FromJson(JsonConvert.SerializeObject(googleCredentialOptions.Value));

            var builder = new TextToSpeechClientBuilder();
            builder.ChannelCredentials = credentials.ToChannelCredentials();
            _client = builder.Build();
        }

        public async ValueTask<ISpeechResult> TranslateTextAsync(GoogleSpeechRequest request)
        {
            var result = new GoogleSpeechResult();

            var chunks = request.Content.Chunk(30).ToArray();

            for (int i = 0; i < chunks.Length; i++)
            {
                if(i > 0)
                    await Task.Delay(60000);

                await Task.WhenAll(chunks[i].Select((c, j) =>
                    QueueText(c, j, request, result)
                ));
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
