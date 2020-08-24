using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SIO.Testing.Abstractions
{
    public abstract class Specification<TResult> : IAsyncLifetime
    {
        protected abstract Task<TResult> Given();
        protected abstract Task When();
        protected Exception Exception { get; private set; }
        protected ExceptionMode ExceptionMode { get; set; }
        protected TResult Result { get; private set; }

        protected readonly IServiceProvider _serviceProvider;

        protected virtual void BuildServices(IServiceCollection services) { }

        public Specification()
        {
            var services = new ServiceCollection();

            BuildServices(services);

            _serviceProvider = services.BuildServiceProvider();
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual async Task InitializeAsync()
        {
            await When();

            try
            {
                Result = await Given();
            }
            catch (Exception e)
            {
                if (ExceptionMode == ExceptionMode.Record)
                    Exception = e;
                else
                    throw;
            }
        }
    }
    public abstract class Specification : IAsyncLifetime
    {
        protected abstract Task Given();
        protected abstract Task When();
        protected Exception Exception { get; private set; }
        protected ExceptionMode ExceptionMode { get; set; }

        protected readonly IServiceProvider _serviceProvider;
        protected virtual void BuildServices(IServiceCollection services) { }

        public Specification()
        {
            var services = new ServiceCollection();

            BuildServices(services);

            _serviceProvider = services.BuildServiceProvider();
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual async Task InitializeAsync()
        {
            await When();

            try
            {
                await Given();
            }
            catch (Exception e)
            {
                if (ExceptionMode == ExceptionMode.Record)
                    Exception = e;
                else
                    throw;
            }
        }
    }
}
