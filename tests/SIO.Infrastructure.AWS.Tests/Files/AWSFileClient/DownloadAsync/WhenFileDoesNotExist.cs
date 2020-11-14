using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using SIO.Infrastructure.AWS.Tests.Translations.AWSSpeechSynthesizer;
using SIO.Infrastructure.Files;
using SIO.Testing.Attributes;
using SIO.Testing.Abstractions;

namespace SIO.Infrastructure.AWS.Tests.Files.AWSFileClient.DownloadAsync
{
    public class WhenFileDoesNotExist : AWSFileClientSpecification<FileResult>
    {
        private readonly string _fileName = $"{Guid.NewGuid()}.txt";
        private readonly string _userId = Guid.NewGuid().ToString();

        public WhenFileDoesNotExist(ConfigurationFixture configurationFixture, FileClientFixture fileClientFixture) : base(configurationFixture, fileClientFixture)
        {
        }

        protected override async Task<FileResult> Given()
        {
            return await FileClient.DownloadAsync(_fileName, _userId);
        }

        protected override Task When()
        {
            ExceptionMode = ExceptionMode.Record;
            return Task.CompletedTask;
        }

        [Integration]
        public void FileNotFoundExceptionShouldBeThrown()
        {
            Exception.Should().BeOfType<FileNotFoundException>();
        }
    }
}
