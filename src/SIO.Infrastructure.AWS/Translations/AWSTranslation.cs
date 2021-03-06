﻿using System;
using System.Threading.Tasks;
using Hangfire;
using OpenEventSourcing.Extensions;
using SIO.Domain.Document.Events;
using SIO.Domain.Translation.Events;
using SIO.Domain.Translations;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.AWS.Translations
{
    public sealed class AWSTranslation : Translation
    {
        private readonly ITranslationWorker<AWSTranslation> _translationWorker;
        private readonly IEventPublisher _eventPublisher;
        public AWSTranslation(ITranslationWorker<AWSTranslation> translationWorker,
            IEventPublisher eventPublisher)
        {
            if (translationWorker == null)
                throw new ArgumentNullException(nameof(translationWorker));
            if (eventPublisher == null)
                throw new ArgumentNullException(nameof(eventPublisher));

            _translationWorker = translationWorker;
            _eventPublisher = eventPublisher;

            Handles<DocumentUploaded>(Handle);
        }

        private async Task Handle(DocumentUploaded documentUploaded)
        {
            if (documentUploaded.TranslationType != TranslationType.AWS)
                return;

            var translationQueuedEvent = new TranslationQueued(
               aggregateId: Guid.NewGuid().ToSequentialGuid(),
               version: documentUploaded.Version + 1,
               correlationId: documentUploaded.AggregateId,
               causationId: documentUploaded.Id,
               userId: documentUploaded.UserId
           );

            await _eventPublisher.PublishAsync(translationQueuedEvent);

            BackgroundJob.Enqueue(() => _translationWorker.StartAsync(
                new TranslationRequest(
                    translationQueuedEvent.AggregateId,
                    documentUploaded.AggregateId,
                    translationQueuedEvent.CausationId.Value,
                    translationQueuedEvent.Version,
                    documentUploaded.UserId,
                    documentUploaded.FileName,
                    documentUploaded.TranslationSubject
                )
            ));
        }
    }
}
