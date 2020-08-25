using System;
using System.Threading.Tasks;
using SIO.Infrastructure.AWS.Translations;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.AWS.Tests.Stubs
{
    internal sealed class InMemoryAWSSpeechSynthesizer : ISpeechSynthesizer<AWSSpeechRequest>
    {
        private readonly bool _throwException;

        public InMemoryAWSSpeechSynthesizer(bool throwException = false)
        {
            _throwException = throwException;
        }

        public ValueTask<ISpeechResult> TranslateTextAsync(AWSSpeechRequest request)
        {
            if (_throwException)
                throw new Exception();

            return new ValueTask<ISpeechResult>(new InMemoryAWSSpeechResult());
        }
    }
}
