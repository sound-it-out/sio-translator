using System;
using System.Collections.Generic;
using System.Text;

namespace SIO.Infrastructure.AWS.Translations
{
    public class AWSTranslationOptions
    {
        public int PollRate { get; set; }
        public string Bucket { get; set; }
    }
}
