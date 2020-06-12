using System;

namespace SIO.Translator.Migrations.Entities
{
    public class TranslatorState
    {
        public string Name { get; set; }
        public long Position { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
    }
}