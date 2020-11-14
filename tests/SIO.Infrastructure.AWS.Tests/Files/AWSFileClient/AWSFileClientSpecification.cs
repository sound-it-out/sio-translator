using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.AWS.Extensions;
using SIO.Infrastructure.AWS.Tests.Translations.AWSSpeechSynthesizer;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Translations;
using SIO.Testing.Abstractions;
using Xunit;

namespace SIO.Infrastructure.AWS.Tests.Files.AWSFileClient
{
    public abstract class AWSFileClientSpecification<TResult> : SpecificationWithConfiguration<ConfigurationFixture, TResult>, IClassFixture<FileClientFixture>
    {
        private readonly Lazy<FileClientFixture> _fileClientFixture;
        protected IFileClient FileClient => _fileClientFixture.Value;

        public AWSFileClientSpecification(ConfigurationFixture configurationFixture, FileClientFixture fileClientFixture) : base(configurationFixture)
        {
            _fileClientFixture = new Lazy<FileClientFixture>(() =>
            {
                fileClientFixture.InitFileClient(_serviceProvider.GetRequiredService<IFileClient>());
                return fileClientFixture;
            });
        }

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);
            services.AddAWSConfiguration(_configurationFixture.Configuration)
                .AddAWSFiles();
        }
    }

    public abstract class AWSFileClientSpecification : SpecificationWithConfiguration<ConfigurationFixture>, IClassFixture<FileClientFixture>
    {
        private readonly Lazy<FileClientFixture> _fileClientFixture;
        protected IFileClient FileClient => _fileClientFixture.Value;

        public AWSFileClientSpecification(ConfigurationFixture configurationFixture, FileClientFixture fileClientFixture) : base(configurationFixture)
        {
            _fileClientFixture = new Lazy<FileClientFixture>(() =>
            {
                fileClientFixture.InitFileClient(_serviceProvider.GetRequiredService<IFileClient>());
                return fileClientFixture;
            });
        }

        protected override void BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);
            services.AddAWSConfiguration(_configurationFixture.Configuration)
                .AddAWSFiles();
        }
    }
}
