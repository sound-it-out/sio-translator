using Clipboard;
using SIO.Translator.Domain.Translation.Events;
using SIO.Translator.Infrastructure.Events;
using SIO.Translator.Infrastructure.Files;
using SIO.Translator.Infrastructure.Translations;
using System;
using System.Threading.Tasks;

namespace SIO.Translator.Infrastructure.AWS.Translations
{
    internal sealed class AWSTranslationWorker : ITranslationWorker<AWSTranslation>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IFileClient _fileClient;
        private readonly ISpeechSynthesizer<AWSSpeechRequest> _speechSynthesizer;

        public AWSTranslationWorker(IEventPublisher eventPublisher,
            IFileClient fileClient,
            ISpeechSynthesizer<AWSSpeechRequest> speechSynthesizer)
        {
            if (eventPublisher == null)
                throw new ArgumentNullException(nameof(eventPublisher));
            if (fileClient == null)
                throw new ArgumentNullException(nameof(fileClient));
            if (speechSynthesizer == null)
                throw new ArgumentNullException(nameof(speechSynthesizer));
        }

        public async Task StartAsync(TranslationRequest request)
        {
            int version = request.Version + 1;

            var fileResult = await _fileClient.DownloadAsync(
                fileName: request.CorrelationId.ToString(),
                userId: request.UserId
            );

            string text;

            using(var fileStream = await fileResult.OpenStreamAsync())
            using(var textExtractor = TextExtractor.Open(fileStream, fileResult.ContentType))
            {
                text = await textExtractor.ExtractAsync();
            }

            await _eventPublisher.PublishAsync(new TranslationStarted(
                aggregateId: request.AggregateId,
                version: version,
                correlationId: request.CorrelationId,
                causationId: null,
                characterCount: text.Length
            ));

            try
            {
                var result = await _speechSynthesizer.TranslateTextAsync(new AWSSpeechRequest { 
                    OutputFormat = "",
                    Text = text,
                    VoiceId = Amazon.Polly.VoiceId.Amy
                });;


                using (var stream = await result.OpenStreamAsync())
                {
                    await _fileClient.UploadAsync($"{request.AggregateId}.mp3", Guid.Empty.ToString(), stream);
                    await _eventPublisher.PublishAsync(new TranslationSucceded(
                        aggregateId: request.AggregateId,
                        version: ++version,
                        correlationId: request.CorrelationId,
                        causationId: null
                    ));
                }
            }
            catch (Exception e)
            {
                await _eventPublisher.PublishAsync(new TranslationFailed(
                    aggregateId: request.AggregateId,
                    version: ++version,
                    correlationId: request.CorrelationId,
                    causationId: null,
                    error: e.Message
                ));
            }
        }
    }
}
