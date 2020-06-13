using System;
using System.Threading.Tasks;
using Hangfire;
using SIO.Domain.Document.Events;
using SIO.Domain.Translations;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.AWS.Translations
{
    internal sealed class AWSTranslation : Translation
    {
        private readonly ITranslationWorker<AWSTranslation> _translationWorker;
        public AWSTranslation(ITranslationWorker<AWSTranslation> translationWorker)
        {
            if (translationWorker == null)
                throw new ArgumentNullException(nameof(translationWorker));

            _translationWorker = translationWorker;

            Handles<DocumentUploaded>(Handle);
        }

        private async Task Handle(DocumentUploaded documentUploaded)
        {
            if (documentUploaded.TranslationType != TranslationType.AWS)
                return;

            BackgroundJob.Enqueue(() => _translationWorker.StartAsync(
                new TranslationRequest (
                    Guid.NewGuid(),
                    documentUploaded.AggregateId,
                    documentUploaded.Version,
                    documentUploaded.UserId,
                    documentUploaded.FileName
                )
            ));
        }
    }
}
