using System;
using System.Threading;
using System.Threading.Tasks;
using SIO.Infrastructure.Google.Translations;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.Google.Tests.Translations.GoogleSpeechSynthesizer
{
    public class SpeechSynthesizerFixture : ISpeechSynthesizer<GoogleSpeechRequest>,  IDisposable
    {
        private ISpeechSynthesizer<GoogleSpeechRequest> _speechSynthesizer;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private Task<ISpeechResult> _getSpeechTask;
        private readonly object _lockObj = new object();

        public void InitSynthesizer(ISpeechSynthesizer<GoogleSpeechRequest> speechSynthesizer) => _speechSynthesizer = speechSynthesizer;

        public void Dispose()
        {
            _cts.Cancel();
        }

        public async ValueTask<ISpeechResult> TranslateTextAsync(GoogleSpeechRequest request)
        {
            lock (_lockObj)
            {
                if (_getSpeechTask == null)
                {
                    _getSpeechTask = Task.Run(async () => await _speechSynthesizer.TranslateTextAsync(request), _cts.Token);
                }
            }

            return await _getSpeechTask;
        }
    }
}
