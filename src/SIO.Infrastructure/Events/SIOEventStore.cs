using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace SIO.Infrastructure.Events
{
    internal sealed class SIOEventStore : ISIOEventStore
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IEventDeserializer _eventDeserializer;
        private readonly IEventTypeCache _eventTypeCache;

        public SIOEventStore(IDbContextFactory dbContextFactory,
                                             IEventDeserializer eventDeserializer,
                                             IEventTypeCache eventTypeCache)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (eventDeserializer == null)
                throw new ArgumentNullException(nameof(eventDeserializer));;
            if (eventTypeCache == null)
                throw new ArgumentNullException(nameof(eventTypeCache));

            _dbContextFactory = dbContextFactory;
            _eventDeserializer = eventDeserializer;
            _eventTypeCache = eventTypeCache;
        }

        public async Task<Page> TryGetEventsAsync(long offset)
        {
            var results = new List<IEvent>();

            using (var context = _dbContextFactory.Create())
            {
                var events = await context.Events
                        .AsNoTracking()
                        .OrderBy(x => x.SequenceNo)
                        .Skip((int)offset)
                        .Take(250)
                        .ToListAsync();

                foreach (var @event in events)
                {
                    if (!_eventTypeCache.TryGet(@event.Type, out var type))
                        continue;

                    var result = (IEvent)_eventDeserializer.Deserialize(@event.Data, type);

                    results.Add(result);
                }

                return new Page(offset + events.Count, offset, results);
            }
        }
    }
}
