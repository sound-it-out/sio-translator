using Google.Cloud.TextToSpeech.V1;
using SIO.Infrastructure.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Google.Translations
{
    public sealed class GoogleSpeechRequest : ISpeechRequest
    {
        public VoiceSelectionParams VoiceSelection { get; }
        public AudioConfig AudioConfig { get; }
        public IEnumerable<string> Content { get; }
        public Func<long, Task> CallBack { get; }

        public GoogleSpeechRequest(VoiceSelectionParams voiceSelection, AudioConfig audioConfig, IEnumerable<string> content, Func<long, Task> callback = null)
        {
            if (voiceSelection == null)
                throw new ArgumentNullException(nameof(voiceSelection));
            if (audioConfig == null)
                throw new ArgumentNullException(nameof(audioConfig));
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            if (!content.Any())
                throw new Exception("Must contain at least one character");

            if (content.Any(s => s.Length > 5000))
                throw new Exception("Google text to speech requests cannot be greater than 5000 characters");

            VoiceSelection = voiceSelection;
            AudioConfig = audioConfig;
            Content = content;
            CallBack = callback;
        }
    }
}
