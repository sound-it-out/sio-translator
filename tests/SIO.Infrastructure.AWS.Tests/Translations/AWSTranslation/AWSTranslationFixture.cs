using System;
using System.Threading;
using System.Threading.Tasks;
using SIO.Domain.Document.Events;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.AWS.Tests.Translations.AWSTranslation
{
    public class AWSTranslationFixture : Translation,  IDisposable
    {
        private AWS.Translations.AWSTranslation _translation;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private Task _handleTask;
        private readonly object _lockObj = new object();

        public AWSTranslationFixture()
        {
            Handles<DocumentUploaded>(Handle);
        }

        public async Task Handle(DocumentUploaded @event)
        {
            lock (_lockObj)
            {
                if (_handleTask == null)
                {
                    _handleTask = Task.Run(async () => await _translation.HandleAsync(@event), _cts.Token);
                }
            }

            await _handleTask;
        }

        public void InitSynthesizer(AWS.Translations.AWSTranslation translation) => _translation = translation;

        public void Dispose()
        {
            _cts.Cancel();
        }
    }
}
