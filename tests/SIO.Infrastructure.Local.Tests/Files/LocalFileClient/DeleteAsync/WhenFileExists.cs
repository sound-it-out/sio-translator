using System;
using System.IO;
using System.Threading.Tasks;
using SIO.Testing.Attributes;

namespace SIO.Infrastructure.Local.Tests.Files.LocalFileClient.DeleteAsync
{
    public class WhenFileExists : LocalFileClientSpecification
    {
        private readonly string _fileName = $"{Guid.NewGuid()}.txt";
        private readonly string _userId = Guid.NewGuid().ToString();
        private const string _text = "some test text.";

        public WhenFileExists(FileClientFixture fileClientFixture) : base(fileClientFixture)
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

        [Then]
        public async Task FileShouldNotExist()
        {
            try
            {
                await FileClient.DownloadAsync(_fileName, _userId);
            }
            catch(FileNotFoundException e)
            {

            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
