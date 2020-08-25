using System.IO;
using System.Threading.Tasks;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.AWS.Tests.Stubs
{
    internal sealed class InMemoryAWSSpeechResult : ISpeechResult
    {
        public ValueTask<Stream> OpenStreamAsync()
        {
            return new ValueTask<Stream>(new MemoryStream());
        }
    }
}
