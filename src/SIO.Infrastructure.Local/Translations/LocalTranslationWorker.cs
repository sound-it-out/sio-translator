using System;
using System.Threading.Tasks;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.Local.Translations
{
    internal class LocalTranslationWorker : ITranslationWorker<LocalTranslation>
    {
        public Task StartAsync(TranslationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
