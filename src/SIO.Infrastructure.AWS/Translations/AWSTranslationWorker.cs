using Clipboard;
using SIO.Domain.Translation.Events;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Translations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SIO.Infrastructure.AWS.Translations
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
                fileName: $"{request.CorrelationId.ToString()}{Path.GetExtension(request.FileName)}",
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
                causationId: Guid.Empty,
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
                    await _fileClient.UploadAsync($"{request.AggregateId}.mp3", request.UserId, stream);
                    await _eventPublisher.PublishAsync(new TranslationSucceded(
                        aggregateId: request.AggregateId,
                        version: ++version,
                        correlationId: request.CorrelationId,
                        causationId: Guid.Empty
                    ));
                }
            }
            catch (Exception e)
            {
                await _eventPublisher.PublishAsync(new TranslationFailed(
                    aggregateId: request.AggregateId,
                    version: ++version,
                    correlationId: request.CorrelationId,
                    causationId: Guid.Empty,
                    error: e.Message
                ));
            }
        }
    }
}
