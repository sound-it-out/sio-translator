using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.EntityFrameworkCore.InMemory;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using SIO.Infrastructure.Files;
using SIO.Migrations.DbContexts;
using SIO.Testing.Stubs;

namespace SIO.Testing.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryFiles(this IServiceCollection source)
        {
            source.AddScoped<IFileClient, InMemoryFileClient>();
            return source;
        }

        public static IServiceCollection AddInMemoryDatabase(this IServiceCollection source)
        {
            source.AddDbContext<SIOTranslatorDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            source.AddOpenEventSourcing()
                .AddEntityFrameworkCoreInMemory();
            return source;
        }

        public static IServiceCollection AddInMemoryEventBus(this IServiceCollection source)
        {
            source.AddScoped<InMemoryEventBus>();
            source.AddScoped<IEventBusPublisher, InMemoryEventBus>(sp => sp.GetRequiredService<InMemoryEventBus>());
            source.AddScoped<IEventBusConsumer, InMemoryEventBus>(sp => sp.GetRequiredService<InMemoryEventBus>());
            return source;
        }
    }
}
