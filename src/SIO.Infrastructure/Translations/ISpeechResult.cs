using System.IO;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Translations
{
    public interface ISpeechResult
    {
        ValueTask<Stream> OpenStreamAsync();
    }
}