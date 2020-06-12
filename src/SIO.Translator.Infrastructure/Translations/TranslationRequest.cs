using System;

namespace SIO.Translator.Infrastructure.Translations
{
    public struct TranslationRequest
    {
        public readonly Guid AggregateId;
        public readonly Guid CorrelationId;
        public readonly int Version;
        public readonly string UserId;

        public TranslationRequest(Guid aggregateId, Guid correlationId, int version, string userId)
        {
            AggregateId = aggregateId;
            CorrelationId = correlationId;
            Version = version;
            UserId = userId;
        }
    }
}
