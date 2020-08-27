using System;
using SIO.Domain.Document.Events;
using SIO.Domain.Translations;

namespace SIO.Infrastructure.AWS.Tests.Translations.AWSTranslation
{
    public class DocumentUploadedFixture : DocumentUploaded
    {
        protected DocumentUploadedFixture(Guid aggregateId, TranslationType translationType, string translationSubject, string fileName) : base(aggregateId, translationType, translationSubject, fileName)
        {
        }

        protected DocumentUploadedFixture(Guid aggregateId, Guid userId, TranslationType translationType, string translationSubject, string fileName) : base(aggregateId, userId, translationType, translationSubject, fileName)
        {
        }

        public DocumentUploadedFixture() : base(Guid.NewGuid(), Guid.NewGuid(), TranslationType.Google, "", "")
        {

        }

        public void Init(TranslationType translationType) => TranslationType = translationType;
    }
}
