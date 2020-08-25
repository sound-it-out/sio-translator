using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Polly;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.AWS.Translations
{
    public class AWSSpeechRequest : ISpeechRequest
    {
        public OutputFormat OutputFormat { get; set; }
        public VoiceId VoiceId { get; set; }
        public IEnumerable<string> Content { get; set; }
        public Func<long, Task> CallBack { get; set; }
    }
}
