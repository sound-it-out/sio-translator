using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticFiles;
using SIO.Infrastructure.Files;

namespace SIO.Testing.Stubs
{
    internal class InMemoryFileClient : IFileClient
    {
        private readonly ConcurrentDictionary<string, FileValue> _files;
        public InMemoryFileClient()
        {
            _files = new ConcurrentDictionary<string, FileValue>();
        }

        public Task DeleteAsync(string fileName, string userId)
        {
            _files.TryRemove(BuildPath(fileName, userId), out var file);
            return Task.CompletedTask;
        }

        public Task<FileResult> DownloadAsync(string fileName, string userId)
        {
            if(_files.TryGetValue(BuildPath(fileName, userId), out var file))
            {
                return Task.FromResult(new FileResult(file.ContentType, () => new MemoryStream(file.Content)));
            }

            throw new InvalidOperationException();
        }

        public Task UploadAsync(string fileName, string userId, Stream stream)
        {
            _files.TryAdd(BuildPath(fileName, userId), new FileValue(fileName, stream));
            return Task.CompletedTask;
        }


        private string BuildPath(string fileName, string userId)
        {
            return Path.Combine(userId, fileName);
        }

        private class FileValue
        {
            public string ContentType { get; set; }
            public byte[] Content { get; set; }

            public FileValue(string fileName, Stream stream)
            {
                var provider = new FileExtensionContentTypeProvider();

                if (!provider.TryGetContentType(fileName, out var contentType))
                {
                    throw new Exception();

                }

                ContentType = contentType;

                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    Content = ms.ToArray();
                }
            }
        }
    }
}
