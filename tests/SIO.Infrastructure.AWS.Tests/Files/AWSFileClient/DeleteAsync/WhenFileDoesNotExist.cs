using System;
using System.Threading.Tasks;
using FluentAssertions;
using SIO.Infrastructure.AWS.Tests.Translations.AWSSpeechSynthesizer;
using SIO.Testing.Abstractions;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.AWS.Tests.Files.AWSFileClient.DeleteAsync
{
    public class WhenFileDoesNotExist : AWSFileClientSpecification
    {
        private readonly string _fileName = $"{Guid.NewGuid()}.txt";
        private readonly string _userId = Guid.NewGuid().ToString();

        public WhenFileDoesNotExist(ConfigurationFixture configurationFixture, FileClientFixture fileClientFixture) : base(configurationFixture, fileClientFixture)
        {
        }

        protected override async Task Given()
        {
            ExceptionMode = ExceptionMode.Record;
            await FileClient.DeleteAsync(_fileName, _userId);
        }

        protected override Task When()
        {
            return Task.CompletedTask;
        }

        [Integration]
        public async Task NoErrorShouldBeThrown()
        {
            Exception.Should().BeNull();
        }
    }
}
