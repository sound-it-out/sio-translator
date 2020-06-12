using System.IO;
using System.Threading.Tasks;

namespace SIO.Translator.Infrastructure.Translations
{
    public interface ISpeechResult
    {
        ValueTask<Stream> OpenStreamAsync();
    }
}