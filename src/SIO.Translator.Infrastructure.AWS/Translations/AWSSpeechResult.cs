using SIO.Translator.Infrastructure.Translations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SIO.Translator.Infrastructure.AWS.Translations
{
    public class AWSSpeechResult : ISpeechResult
    {
        private readonly Func<ValueTask<Stream>> _stream;

        public AWSSpeechResult(Func<ValueTask<Stream>> func)
        {
            _stream = func;
        }
        public ValueTask<Stream> OpenStreamAsync()
        {
            return _stream.Invoke();
        }
    }
}
