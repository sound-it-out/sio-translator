using System.Threading.Tasks;
using SIO.Infrastructure.AWS.Translations;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.AWS.Tests.Stubs
{
    internal sealed class InMemoryAWSTranslationWorker : ITranslationWorker<AWSTranslation>
    {
        public Task StartAsync(TranslationRequest request)
        {
            return Task.CompletedTask;
        }
    }
}
