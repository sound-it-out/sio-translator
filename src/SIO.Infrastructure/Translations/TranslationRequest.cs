using System;

namespace SIO.Infrastructure.Translations
{
    public struct TranslationRequest
    {
        public readonly Guid AggregateId;
        public readonly Guid CorrelationId;
        public readonly int Version;
        public readonly string UserId;
        public readonly string FileName;

        public TranslationRequest(Guid aggregateId, Guid correlationId, int version, string userId, string fileName)
        {
            AggregateId = aggregateId;
            CorrelationId = correlationId;
            Version = version;
            UserId = userId;
            FileName = fileName;
        }
    }
}
