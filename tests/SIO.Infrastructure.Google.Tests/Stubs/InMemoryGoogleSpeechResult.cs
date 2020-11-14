using System.IO;
using System.Threading.Tasks;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.Google.Tests.Stubs
{
    internal sealed class InMemoryGoogleSpeechResult : ISpeechResult
    {
        public ValueTask<Stream> OpenStreamAsync()
        {
            return new ValueTask<Stream>(new MemoryStream());
        }
    }
}
