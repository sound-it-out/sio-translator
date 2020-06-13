using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Events;
using SIO.Infrastructure.Translations;
using SIO.Migrations.DbContexts;
using SIO.Migrations.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Domain.Translations
{
    public class BackgroundTranslator<TTranslator> : IHostedService
        where TTranslator : ITranslation
    {
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        protected readonly ILogger<BackgroundTranslator<TTranslator>> _logger;
        protected readonly IServiceScope _scope;
        private readonly IEventStore _eventStore;
        private readonly TTranslator _translator;

        protected TranslatorState _translatorState;
        private Task _executingTask;

        public BackgroundTranslator(ILogger<BackgroundTranslator<TTranslator>> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));

            _logger = logger;
            _scope = serviceScopeFactory.CreateScope();
            _eventStore = _scope.ServiceProvider.GetRequiredService<IEventStore>();
            _translator = _scope.ServiceProvider.GetRequiredService<TTranslator>();
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            if (_executingTask.IsCompleted)
                return _executingTask;

            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _scope.Dispose();
            if (_executingTask == null)
                return;

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
        }

        private async Task PollAsync(CancellationToken cancellationToken)
        {
            var translatorName = typeof(TTranslator).FullName;
            using (var context = _scope.ServiceProvider.GetRequiredService<SIOTranslatorDbContext>())
            {
                _translatorState = context.TranslatorStates.FirstOrDefault(ps => ps.Name == translatorName);

                if (_translatorState == null)
                {
                    _translatorState = new TranslatorState
                    {
                        Name = translatorName,
                        CreatedDate = DateTimeOffset.UtcNow,
                        Position = 0
                    };

                    context.TranslatorStates.Add(_translatorState);

                    await context.SaveChangesAsync();
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await context.Entry(_translatorState).ReloadAsync();

                        var page = await _eventStore.GetEventsAsync(_translatorState.Position);

                        foreach (var @event in page.Events)
                            await _translator.HandleAsync(@event);

                        if (_translatorState.Position == page.Offset)
                        {
                            await Task.Delay(500);
                        }
                        else
                        {
                            _translatorState.Position = page.Offset;
                            _translatorState.LastModifiedDate = DateTimeOffset.UtcNow;

                            await context.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, $"Projection '{typeof(TTranslator).Name}' failed at postion '{_translatorState.Position}' due to an unexpected error. See exception details for more information.");
                        break;
                    }
                }
            }
        }

        protected Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => PollAsync(cancellationToken));
        }
    }
}
