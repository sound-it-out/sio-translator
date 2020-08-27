using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using SIO.Infrastructure.Files;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.Local.Tests.Files.LocalFileClient.DownloadAsync
{
    public class WhenFileExists : LocalFileClientSpecification<FileResult>
    {
        private readonly string _fileName = $"{Guid.NewGuid()}.txt";
        private readonly string _contentType = "text/plain";
        private readonly string _userId = Guid.NewGuid().ToString();
        private const string _text = "some test text.";

        public WhenFileExists(FileClientFixture fileClientFixture) : base(fileClientFixture)
        {
        }

        protected override async Task<FileResult> Given()
        {
            return await FileClient.DownloadAsync(_fileName, _userId);
        }

        protected override async Task When()
        {
            using (var ms = new MemoryStream())
            using(TextWriter tw = new StreamWriter(ms))
            {
                await tw.WriteAsync(_text);
                await tw.FlushAsync();
                ms.Position = 0;
                await FileClient.UploadAsync(_fileName, _userId, ms);
            }            
        }

        [Then]
        public async Task FileShouldContainExpectedText()
        {
            using (var stream = await Result.OpenStreamAsync())
            using(TextReader tr = new StreamReader(stream))
            {
                var text = await tr.ReadToEndAsync();
                text.Should().Be(_text);
            }
        }

        [Then]
        public void FileShouldHaveExpectedContentType()
        {
            Result.ContentType.Should().Be(_contentType);
        }

        public override async Task DisposeAsync()
        {
            await FileClient.DeleteAsync(_fileName, _userId);
            await base.DisposeAsync();
        }
    }
}
