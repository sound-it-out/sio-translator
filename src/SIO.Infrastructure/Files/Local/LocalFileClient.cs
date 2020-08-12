using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Files.Local
{
    internal class LocalFileClient : IFileClient
    {
        public static string Extract(string filename)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filename, out var contentType))
            {
                throw new Exception();

            }
            return contentType;
        }

        public Task<FileResult> DownloadAsync(string fileName, string userId)
        {
            var stream = File.OpenRead(Path.Combine(Path.GetTempPath(), $"sio/{userId}/{fileName}"));
            return Task.FromResult(new FileResult(Extract(stream.Name), () => stream));
        }

        public async Task UploadAsync(string fileName, string userId, Stream stream)
        {
            var path = Path.Combine(Path.GetTempPath(), $"sio/{userId}");
            Directory.CreateDirectory(path);

            using (var s = File.OpenWrite(Path.Combine(path, fileName)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(s);
            }
        }
    }
}
