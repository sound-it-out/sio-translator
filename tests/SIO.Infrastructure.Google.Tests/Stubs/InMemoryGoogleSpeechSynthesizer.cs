using System;
using System.Threading.Tasks;
using SIO.Infrastructure.Google.Translations;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.Google.Tests.Stubs
{
    internal sealed class InMemoryGoogleSpeechSynthesizer : ISpeechSynthesizer<GoogleSpeechRequest>
    {
        private readonly bool _throwException;

        public InMemoryGoogleSpeechSynthesizer(bool throwException = false)
        {
            _throwException = throwException;
        }

        public ValueTask<ISpeechResult> TranslateTextAsync(GoogleSpeechRequest request)
        {
            if (_throwException)
                throw new Exception();

            return new ValueTask<ISpeechResult>(new InMemoryGoogleSpeechResult());
        }
    }
}
