using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using SIO.Infrastructure.Files;
using SIO.Testing.Abstractions;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.Local.Tests.Files.LocalFileClient.DownloadAsync
{
    public class WhenFileDoesNotExist : LocalFileClientSpecification<FileResult>
    {
        private readonly string _fileName = $"{Guid.NewGuid()}.txt";
        private readonly string _userId = Guid.NewGuid().ToString();

        public WhenFileDoesNotExist(FileClientFixture fileClientFixture) : base(fileClientFixture)
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

        [Then]
        public void FileNotFoundExceptionShouldBeThrown()
        {
            Exception.Should().BeOfType<FileNotFoundException>();
        }
    }
}
