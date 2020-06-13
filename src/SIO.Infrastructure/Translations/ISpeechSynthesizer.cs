using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Translations
{
    public interface ISpeechSynthesizer<TRequest>
        where TRequest : ISpeechRequest
    {
        ValueTask<ISpeechResult> TranslateTextAsync(TRequest request);
    }
}
