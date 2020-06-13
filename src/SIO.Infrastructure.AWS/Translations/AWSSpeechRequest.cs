using Amazon.Polly;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.AWS.Translations
{
    public class AWSSpeechRequest : ISpeechRequest
    {
        public OutputFormat OutputFormat { get; set; }
        public VoiceId VoiceId { get; set; }
        public string Text { get; set; }
    }
}
