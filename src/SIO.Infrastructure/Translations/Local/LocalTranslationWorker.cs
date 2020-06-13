using System;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Translations.Local
{
    internal class LocalTranslationWorker : ITranslationWorker<LocalTranslation>
    {
        public Task StartAsync(TranslationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
