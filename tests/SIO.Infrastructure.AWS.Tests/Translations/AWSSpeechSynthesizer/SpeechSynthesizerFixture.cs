using System;
using System.Threading;
using System.Threading.Tasks;
using SIO.Infrastructure.AWS.Translations;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.AWS.Tests.Translations.AWSSpeechSynthesizer
{
    public class SpeechSynthesizerFixture : ISpeechSynthesizer<AWSSpeechRequest>,  IDisposable
    {
        private ISpeechSynthesizer<AWSSpeechRequest> _speechSynthesizer;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private Task<ISpeechResult> _getSpeechTask;
        private readonly object _lockObj = new object();

        public void InitSynthesizer(ISpeechSynthesizer<AWSSpeechRequest> speechSynthesizer) => _speechSynthesizer = speechSynthesizer;

        public void Dispose()
        {
            _cts.Cancel();
        }

        public async ValueTask<ISpeechResult> TranslateTextAsync(AWSSpeechRequest request)
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
