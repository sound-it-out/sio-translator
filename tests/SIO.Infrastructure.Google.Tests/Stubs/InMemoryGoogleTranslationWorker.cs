using System.Threading.Tasks;
using SIO.Infrastructure.Google.Translations;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.Google.Tests.Stubs
{
    internal sealed class InMemoryGoogleTranslationWorker : ITranslationWorker<GoogleTranslation>
    {
        public Task StartAsync(TranslationRequest request)
        {
            return Task.CompletedTask;
        }
    }
}
