using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace SIO.Infrastructure.Events
{
    internal interface ISIOEventStore
    {
        Task<Page> TryGetEventsAsync(long offset);
    }
}
