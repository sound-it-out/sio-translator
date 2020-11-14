using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Clipboard;
using SIO.Domain.Translation.Events;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.Local.Translations
{
    internal class LocalTranslationWorker : ITranslationWorker<LocalTranslation>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IFileClient _fileClient;

        public LocalTranslationWorker(IEventPublisher eventPublisher,
            IFileClient fileClient)
        {
            if (eventPublisher == null)
                throw new ArgumentNullException(nameof(eventPublisher));
            if (fileClient == null)
                throw new ArgumentNullException(nameof(fileClient));

            _eventPublisher = eventPublisher;
            _fileClient = fileClient;
        }

        public async Task StartAsync(TranslationRequest request)
        {
            int version = request.Version + 1;

            var fileResult = await _fileClient.DownloadAsync(
                fileName: $"{request.CorrelationId}{Path.GetExtension(request.FileName)}",
                userId: request.UserId
            );

            string text;

            using (var fileStream = await fileResult.OpenStreamAsync())
            using (var textExtractor = TextExtractor.Open(fileStream, fileResult.ContentType))
            {
                text = await textExtractor.ExtractAsync();
            }

            var textChunks = text.ChunkWithDelimeters(5000, '.', '!', '?', ')', '"', '}', ']').ToArray();

            await _eventPublisher.PublishAsync(new TranslationStarted(
                aggregateId: request.AggregateId,
                version: version,
                correlationId: request.CorrelationId,
                causationId: request.CausationId,
                characterCount: textChunks.Sum(tc => tc.Length),
                userId: request.UserId
            ));

            try
            {
                for (int i = 0; i < textChunks.Count(); i++)
                {
                    version++;
                    await _eventPublisher.PublishAsync(new TranslationCharactersProcessed(
                        aggregateId: request.AggregateId,
                        version: version,
                        correlationId: request.CorrelationId,
                        causationId: request.CausationId,
                        charactersProcessed: textChunks[i].Length,
                        userId: request.UserId
                    ));
                }

                using (var stream = new MemoryStream())
                using (TextWriter tw = new StreamWriter(stream))
                {
                    await tw.WriteAsync("test");
                    await tw.FlushAsync();
                    stream.Position = 0;
                    await _fileClient.UploadAsync($"{request.AggregateId}.mp3", request.UserId, stream);
                    await _eventPublisher.PublishAsync(new TranslationSucceded(
                        aggregateId: request.AggregateId,
                        version: ++version,
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
                    version: ++version,
                    correlationId: request.CorrelationId,
                    causationId: request.CausationId,
                    error: e.Message,
                    userId: request.UserId
                ));
            }
        }
    }
}
