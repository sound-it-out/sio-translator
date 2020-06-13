using Clipboard;
using Google.Cloud.TextToSpeech.V1;
using SIO.Domain.Translation.Events;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Translations;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Google.Translations
{
    internal sealed class GoogleTranslationWorker : ITranslationWorker<GoogleTranslation>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IFileClient _fileClient;
        private readonly ISpeechSynthesizer<GoogleSpeechRequest> _speechSynthesizer;

        public GoogleTranslationWorker(IEventPublisher eventPublisher,
            IFileClient fileClient,
            ISpeechSynthesizer<GoogleSpeechRequest> speechSynthesizer)
        {
            if (eventPublisher == null)
                throw new ArgumentNullException(nameof(eventPublisher));
            if (fileClient == null)
                throw new ArgumentNullException(nameof(fileClient));
            if (speechSynthesizer == null)
                throw new ArgumentNullException(nameof(speechSynthesizer));

            _eventPublisher = eventPublisher;
            _fileClient = fileClient;
            _speechSynthesizer = speechSynthesizer;
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

            var textChunks = text.ChunkWithDelimeters(5000, '.', '!', '?', ')', '"', '}', ']');

            await _eventPublisher.PublishAsync(new TranslationStarted(
                aggregateId: request.AggregateId,
                version: version,
                correlationId: request.CorrelationId,
                causationId: Guid.Empty,
                characterCount: textChunks.Sum(tc => tc.Length)
            ));

            try
            {
                var result = await _speechSynthesizer.TranslateTextAsync(new GoogleSpeechRequest(
                    voiceSelection: new VoiceSelectionParams
                    {
                        LanguageCode = "en-US",
                        SsmlGender = SsmlVoiceGender.Neutral
                    },
                    audioConfig: new AudioConfig
                    {
                        AudioEncoding = AudioEncoding.Mp3
                    },
                    content: textChunks,
                    callback: async length =>
                    {
                        Interlocked.Increment(ref version);

                        await _eventPublisher.PublishAsync(new TranslationCharactersProcessed(
                            aggregateId: request.AggregateId,
                            version: version,
                            correlationId: request.CorrelationId,
                            causationId: Guid.Empty,
                            charactersProcessed: length
                        ));
                    }
                ));


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
                    causationId: null,
                    error: e.Message
                ));
            }
        }
    }
}
