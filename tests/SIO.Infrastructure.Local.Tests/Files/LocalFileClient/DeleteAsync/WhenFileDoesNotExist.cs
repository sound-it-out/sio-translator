using System;
using System.Threading.Tasks;
using FluentAssertions;
using SIO.Testing.Abstractions;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.Local.Tests.Files.LocalFileClient.DeleteAsync
{
    public class WhenFileDoesNotExist : LocalFileClientSpecification
    {
        private readonly string _fileName = $"{Guid.NewGuid()}.txt";
        private readonly string _userId = Guid.NewGuid().ToString();

        public WhenFileDoesNotExist(FileClientFixture fileClientFixture) : base(fileClientFixture)
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

        [Then]
        public async Task NoErrorShouldBeThrown()
        {
            Exception.Should().BeNull();
        }
    }
}
