using SIO.Domain.Translations;
using System;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Translations
{
    public interface ITranslationWorker<TTranslation>
        where TTranslation : ITranslation
    {
        Task StartAsync(TranslationRequest request);
    }
}
