using Hangfire;
using SIO.Translator.Domain.Translation.Events;
using SIO.Translator.Domain.Translations;
using SIO.Translator.Infrastructure.Translations;
using System;
using System.Threading.Tasks;

namespace SIO.Translator.Infrastructure.Google.Translations
{
    internal sealed class GoogleTranslation : Translation
    {
        private readonly ITranslationWorker<GoogleTranslation> _translationWorker;
        public GoogleTranslation(ITranslationWorker<GoogleTranslation> translationWorker)
        {
            if (translationWorker == null)
                throw new ArgumentNullException(nameof(translationWorker));

            _translationWorker = translationWorker;

            Handles<DocumentUploaded>(Handle);
        }

        private async Task Handle(DocumentUploaded documentUploaded)
        {
            if (documentUploaded.TranslationType != TranslationType.Google)
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
