using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.StaticFiles;
using SIO.Translator.Infrastructure.Files;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SIO.Translator.Infrastructure.AWS.Files
{
    internal class AWSFileClient : IFileClient
    {
        private readonly IAmazonS3 _client;

        public AWSFileClient()
        {
            _client = new AmazonS3Client(new BasicAWSCredentials("", ""));
        }

        public async Task<FileResult> DownloadAsync(string fileName, string userId)
        {
            var file = await _client.GetObjectAsync(userId, fileName);
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return new FileResult(contentType, () =>  file.ResponseStream);
        }

        public async Task UploadAsync(string fileName, string userId, Stream stream)
        {
            await _client.UploadObjectFromStreamAsync(userId, fileName, stream, new Dictionary<string, object>());
        }
    }
}
