using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenEventSourcing.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Events
{
    internal class EventConsumer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public EventConsumer(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
                return scope.ServiceProvider.GetRequiredService<IEventBusConsumer>().StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
                return scope.ServiceProvider.GetRequiredService<IEventBusConsumer>().StopAsync(cancellationToken);
        }
    }
}
