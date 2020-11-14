using System.Threading.Tasks;
using SIO.Infrastructure.Local.Translations;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.Local.Tests.Stubs
{
    internal sealed class InMemoryLocalTranslationWorker : ITranslationWorker<LocalTranslation>
    {
        public Task StartAsync(TranslationRequest request)
        {
            return Task.CompletedTask;
        }
    }
}
