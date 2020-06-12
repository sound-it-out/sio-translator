using System;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;

namespace SIO.Translator.Domain.Translation.Events
{
    public class TranslationSucceded : IEvent
    {
        public Guid Id { get; }
        public Guid AggregateId { get; }
        public Guid? CorrelationId { get; }
        public Guid? CausationId { get; }
        public DateTimeOffset Timestamp { get; }
        public int Version { get; }
        public string UserId { get; }


        public TranslationSucceded(Guid aggregateId, int version, Guid? correlationId, Guid? causationId)
        {
            Id = Guid.NewGuid().ToSequentialGuid();
            AggregateId = aggregateId;
            CorrelationId = correlationId;
            CausationId = causationId;
            Timestamp = DateTimeOffset.UtcNow;
            Version = version;
            UserId = Guid.Empty.ToString();
        }

        public void UpdateFrom(ICommand command)
        {
            throw new NotImplementedException();
        }
    }
}
