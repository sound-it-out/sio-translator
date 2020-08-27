using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticFiles;
using SIO.Infrastructure.Files;

namespace SIO.Infrastructure.Local.Files
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

        public Task DeleteAsync(string fileName, string userId)
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"sio/{userId}/{fileName}");

            if (File.Exists(filePath))
                File.Delete(filePath);

            return Task.CompletedTask;
        }

        public Task<FileResult> DownloadAsync(string fileName, string userId)
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"sio/{userId}/{fileName}");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Unable to find file: {fileName}");

            var stream = File.OpenRead(filePath);
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
