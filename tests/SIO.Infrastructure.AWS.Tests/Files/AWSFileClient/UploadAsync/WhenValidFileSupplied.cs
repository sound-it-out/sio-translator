using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using SIO.Infrastructure.AWS.Tests.Translations.AWSSpeechSynthesizer;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.AWS.Tests.Files.AWSFileClient.UploadAsync
{
    public class WhenValidFileSupplied : AWSFileClientSpecification
    {
        private readonly string _fileName = $"{Guid.NewGuid()}.txt";
        private readonly string _contentType = "text/plain";
        private readonly string _userId = Guid.NewGuid().ToString();
        private const string _text = "some test text.";

        public WhenValidFileSupplied(ConfigurationFixture configurationFixture, FileClientFixture fileClientFixture) : base(configurationFixture, fileClientFixture)
        {
        }

        protected override async Task Given()
        {
            using(var ms = new MemoryStream())
            using (TextWriter tw = new StreamWriter(ms))
            {
                await tw.WriteAsync(_text);
                await tw.FlushAsync();
                ms.Position = 0;
                await FileClient.UploadAsync(_fileName, _userId, ms);
            }     
        }

        protected override Task When()
        {
            return Task.CompletedTask;
        }

        [Integration]
        public async Task UploadedFileShouldContainExpectedText()
        {
            var file = await FileClient.DownloadAsync(_fileName, _userId);
            using (var stream = await file.OpenStreamAsync())
            using (TextReader tr = new StreamReader(stream))
            {
                var text = await tr.ReadToEndAsync();
                text.Should().Be(_text);
            }
        }

        [Integration]
        public async Task UploadedShouldHaveExpectedContentType()
        {
            var file = await FileClient.DownloadAsync(_fileName, _userId);
            file.ContentType.Should().Be(_contentType);
        }

        public override async Task DisposeAsync()
        {
            //await _memoryStream.DisposeAsync();
            await FileClient.DeleteAsync(_fileName, _userId);
            await base.DisposeAsync();
        }
    }
}
