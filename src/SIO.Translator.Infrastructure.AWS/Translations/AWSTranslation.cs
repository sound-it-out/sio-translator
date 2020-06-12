using Hangfire;
using SIO.Translator.Domain.Translation.Events;
using SIO.Translator.Domain.Translations;
using SIO.Translator.Infrastructure.Translations;
using System;
using System.Threading.Tasks;

namespace SIO.Translator.Infrastructure.AWS.Translations
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
                    documentUploaded.UserId
                )
            ));
        }
    }
}
