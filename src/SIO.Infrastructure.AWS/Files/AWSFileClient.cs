using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using SIO.Infrastructure.Files;

namespace SIO.Infrastructure.AWS.Files
{
    internal class AWSFileClient : IFileClient
    {
        private readonly IAmazonS3 _client;
        private readonly string _bucket;

        public AWSFileClient(IOptions<AWSCredentialOptions> awsCredentialOptions, IOptions<AWSFileOptions> fileOptions)
        {
            if (awsCredentialOptions == null)
                throw new ArgumentNullException(nameof(awsCredentialOptions));
            if (fileOptions == null)
                throw new ArgumentNullException(nameof(fileOptions));

            _client = new AmazonS3Client(new BasicAWSCredentials(awsCredentialOptions.Value.AccessKey, awsCredentialOptions.Value.SecretKey), RegionEndpoint.EUWest2);
            _bucket = fileOptions.Value.Bucket;
        }

        public async Task DeleteAsync(string fileName, string userId)
        {
            await _client.DeleteAsync(_bucket, Path.Combine(userId, fileName), new Dictionary<string, object>());
        }

        public async Task<FileResult> DownloadAsync(string fileName, string userId)
        {
            try
            {
                var file = await _client.GetObjectAsync(_bucket, Path.Combine(userId, fileName));
                var provider = new FileExtensionContentTypeProvider();

                if (!provider.TryGetContentType(fileName, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                return new FileResult(contentType, () => file.ResponseStream);
            }
            catch (Exception e)
            {
                throw new FileNotFoundException($"file: {fileName} not found", e);
            }
        }

        public async Task UploadAsync(string fileName, string userId, Stream stream)
        {
            await _client.UploadObjectFromStreamAsync(_bucket, Path.Combine(userId, fileName), stream, new Dictionary<string, object>());
        }
    }
}
