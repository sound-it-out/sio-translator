using OpenEventSourcing.Events;
using System.Threading.Tasks;

namespace SIO.Domain.Translations
{
    public interface ITranslation
    {
        Task HandleAsync(IEvent @event);
    }
}
