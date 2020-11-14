using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SIO.Infrastructure.AWS.Tests.Translations.AWSSpeechSynthesizer;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.AWS.Tests.Files.AWSFileClient.DeleteAsync
{
    public class WhenFileExists : AWSFileClientSpecification
    {
        private readonly string _fileName = $"{Guid.NewGuid()}.txt";
        private readonly string _contentType = "text/plain";
        private readonly string _userId = Guid.NewGuid().ToString();
        private const string _text = "some test text.";

        public WhenFileExists(ConfigurationFixture configurationFixture, FileClientFixture fileClientFixture) : base(configurationFixture, fileClientFixture)
        {
        }

        protected override async Task Given()
        {
            await FileClient.DeleteAsync(_fileName, _userId);
        }

        protected override async Task When()
        {
            using (var ms = new MemoryStream())
            using (TextWriter tw = new StreamWriter(ms))
            {
                await tw.WriteAsync(_text);
                await tw.FlushAsync();
                ms.Position = 0;
                await FileClient.UploadAsync(_fileName, _userId, ms);
            }
        }

        [Integration]
        public async Task FileShouldNotExist()
        {
            try
            {
                await FileClient.DownloadAsync(_fileName, _userId);
            }
            catch(FileNotFoundException e)
            {

            }
            catch(Exception e)
            {
                throw;
            }
        }
    }
}
