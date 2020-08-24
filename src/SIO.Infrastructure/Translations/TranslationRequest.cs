using System;

namespace SIO.Infrastructure.Translations
{
    public class TranslationRequest
    {
        public Guid AggregateId { get; }
        public Guid CorrelationId { get; }
        public Guid CausationId { get; }
        public int Version { get; }
        public string UserId { get; }
        public string FileName { get; }
        public string TranslationSubject { get; }

        public TranslationRequest(Guid aggregateId, Guid correlationId, Guid causationId, int version, string userId, string fileName, string translationSubject)
        {
            AggregateId = aggregateId;
            CorrelationId = correlationId;
            CausationId = causationId;
            Version = version;
            UserId = userId;
            FileName = fileName;
            TranslationSubject = translationSubject;
        }
    }
}
