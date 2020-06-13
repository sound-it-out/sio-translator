using Hangfire;
using OpenEventSourcing.Events;
using SIO.Domain.Translations;
using System;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Translations.Local
{
    internal sealed class LocalTranslation : ITranslation
    {
        private readonly ITranslationWorker<LocalTranslation> _translationWorker;
        public LocalTranslation(ITranslationWorker<LocalTranslation> translationWorker)
        {
            if (translationWorker == null)
                throw new ArgumentNullException(nameof(translationWorker));

            _translationWorker = translationWorker;
        }

        public Task HandleAsync(IEvent @event)
        {
            BackgroundJob.Enqueue(() => _translationWorker.StartAsync(
                new TranslationRequest(
                    Guid.NewGuid(),
                    @event.AggregateId,
                    @event.Version,
                    @event.UserId,
                    ""
                )
            ));

            return Task.CompletedTask;
        }
    }
}
