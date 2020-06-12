using Clipboard;
using Google.Cloud.TextToSpeech.V1;
using SIO.Translator.Domain.Translation.Events;
using SIO.Translator.Infrastructure.Events;
using SIO.Translator.Infrastructure.Extensions;
using SIO.Translator.Infrastructure.Files;
using SIO.Translator.Infrastructure.Translations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Translator.Infrastructure.Google.Translations
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

            var textChunks = text.ChunkWithDelimeters(5000, '.', '!', '?', ')', '"', '}', ']');

            await _eventPublisher.PublishAsync(new TranslationStarted(
                aggregateId: request.AggregateId,
                version: version,
                correlationId: request.CorrelationId,
                causationId: null,
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
                            causationId: null,
                            charactersProcessed: length
                        ));
                    }
                ));


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
