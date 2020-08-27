using Newtonsoft.Json;
using OpenEventSourcing.Events;
using SIO.Domain.Translations;
using System;

namespace SIO.Domain.Document.Events
{
    public class DocumentUploaded : Event
    {
        public TranslationType TranslationType { get; set; }
        public string TranslationSubject { get; set; }
        public string FileName { get; set; }
        public DocumentUploaded(Guid aggregateId, TranslationType translationType, string translationSubject, string fileName) : base(aggregateId, 1)
        {
            TranslationType = translationType;
            TranslationSubject = translationSubject;
            FileName = fileName;
        }
        
        public DocumentUploaded(Guid aggregateId, Guid userId, TranslationType translationType, string translationSubject, string fileName) : base(aggregateId, 1)
        {
            TranslationType = translationType;
            TranslationSubject = translationSubject;
            FileName = fileName;
            UserId = userId.ToString();
        }

        [JsonConstructor]
        public DocumentUploaded(Guid id, Guid aggregateId, Guid userId, TranslationType translationType, string translationSubject, string fileName) : base(aggregateId, 1)
        {
            TranslationType = translationType;
            TranslationSubject = translationSubject;
            FileName = fileName;
            UserId = userId.ToString();
            Id = id;
        }
    }
}
