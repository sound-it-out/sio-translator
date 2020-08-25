using Clipboard;
using SIO.Domain.Translation.Events;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Translations;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.AWS.Translations
{
    internal sealed class AWSTranslationWorker : ITranslationWorker<AWSTranslation>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IFileClient _fileClient;
        private readonly ISpeechSynthesizer<AWSSpeechRequest> _speechSynthesizer;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

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

            var textChunks = text.ChunkWithDelimeters(100000, '.', '!', '?', ')', '"', '}', ']');

            await _eventPublisher.PublishAsync(new TranslationStarted(
                aggregateId: request.AggregateId,
                version: version,
                correlationId: request.CorrelationId,
                causationId: request.CausationId,
                characterCount: text.Length,
                userId: request.UserId
            ));

            try
            {
                var result = await _speechSynthesizer.TranslateTextAsync(new AWSSpeechRequest { 
                    OutputFormat = "",
                    Content = textChunks,
                    VoiceId = request.TranslationSubject,
                    CallBack = async length =>
                    {
                        Interlocked.Increment(ref version);
                        await _semaphoreSlim.WaitAsync();

                        try
                        {
                            await _eventPublisher.PublishAsync(new TranslationCharactersProcessed(
                                aggregateId: request.AggregateId,
                                version: version,
                                correlationId: request.CorrelationId,
                                causationId: request.CausationId,
                                charactersProcessed: length,
                                userId: request.UserId
                            ));
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            _semaphoreSlim.Release();
                        }
                    }
            });;


                using (var stream = await result.OpenStreamAsync())
                {
                    await _fileClient.UploadAsync($"{request.AggregateId}.mp3", request.UserId, stream);
                    await _eventPublisher.PublishAsync(new TranslationSucceded(
                        aggregateId: request.AggregateId,
                        version: version + 1,
                        correlationId: request.CorrelationId,
                        causationId: request.CausationId,
                        userId: request.UserId
                    ));
                }
            }
            catch (Exception e)
            {
                await _eventPublisher.PublishAsync(new TranslationFailed(
                    aggregateId: request.AggregateId,
                    version: version + 1,
                    correlationId: request.CorrelationId,
                    causationId: request.CausationId,
                    error: e.Message,
                    userId: request.UserId
                ));
            }
        }
    }
}
