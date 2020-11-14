using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SIO.Infrastructure.Files;

namespace SIO.Infrastructure.Local.Tests.Files.LocalFileClient
{
    public class FileClientFixture : IFileClient,  IDisposable
    {
        private IFileClient _fileClient;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private Task<FileResult> _downloadTask;
        private Task _deleteTask;
        private Task _uploadTask;
        private readonly object _uploadLockObj = new object();
        private readonly object _downloadLockObj = new object();
        private readonly object _deleteLockObj = new object();

        public void InitFileClient(IFileClient fileClient) => _fileClient = fileClient;

        public void Dispose()
        {
            _cts.Cancel();
        }

        public async Task UploadAsync(string fileName, string userId, Stream stream)
        {
            lock (_uploadLockObj)
            {
                if (_uploadTask == null)
                {
                    _uploadTask = Task.Run(async () => await _fileClient.UploadAsync(fileName, userId, stream), _cts.Token);
                }
            }

            await _uploadTask;
        }

        public async Task<FileResult> DownloadAsync(string fileName, string userId)
        {
            lock (_downloadLockObj)
            {
                if (_downloadTask == null)
                {
                    _downloadTask = Task.Run(async () => await _fileClient.DownloadAsync(fileName, userId), _cts.Token);
                }
            }

            return await _downloadTask;
        }

        public async Task DeleteAsync(string fileName, string userId)
        {
            lock (_deleteLockObj)
            {
                if (_deleteTask == null)
                {
                    _deleteTask = Task.Run(async () => await _fileClient.DeleteAsync(fileName, userId), _cts.Token);
                }
            }

            await _deleteTask;
        }
    }
}
