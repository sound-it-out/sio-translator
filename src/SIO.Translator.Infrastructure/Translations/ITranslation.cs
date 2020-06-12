using OpenEventSourcing.Events;
using System.Threading.Tasks;

namespace SIO.Translator.Domain.Translations
{
    public interface ITranslation
    {
        Task HandleAsync(IEvent @event);
    }
}
