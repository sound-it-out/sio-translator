using OpenEventSourcing.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
        Task PublishAsync(IEnumerable<IEvent> events);
    }
}
