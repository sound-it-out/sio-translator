using OpenEventSourcing.Events;
using SIO.Translator.Domain.Translations;
using System;

namespace SIO.Translator.Domain.Translation.Events
{
    public class DocumentUploaded : Event
    {
        public TranslationType TranslationType { get; set; }
        public string FileName { get; set; }
        public DocumentUploaded(Guid aggregateId, TranslationType translationType, string fileName) : base(aggregateId, 1)
        {
            TranslationType = translationType;
            FileName = fileName;
        }

        public DocumentUploaded(Guid aggregateId, Guid userId, TranslationType translationType, string fileName) : base(aggregateId, 1)
        {
            TranslationType = translationType;
            FileName = fileName;
            UserId = userId.ToString();
        }
    }
}
