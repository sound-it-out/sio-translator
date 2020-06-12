using OpenEventSourcing.Events;
using SIO.Translator.Domain.Translations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIO.Translator.Infrastructure.Translations
{
    public abstract class Translation : ITranslation
    {
        private readonly Dictionary<Type, Func<IEvent, Task>> _handlers;

        protected Translation()
        {
            _handlers = new Dictionary<Type, Func<IEvent, Task>>();
        }

        protected void Handles<TEvent>(Func<TEvent, Task> handler)
            where TEvent : IEvent
        {
            _handlers.Add(typeof(TEvent), e => handler((TEvent)e));
        }

        public async Task HandleAsync(IEvent @event)
        {
            if (_handlers.TryGetValue(@event.GetType(), out var handler))
                await handler(@event);
        }
    }
}
