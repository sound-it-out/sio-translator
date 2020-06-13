using System;
using System.IO;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Files
{
    public class FileResult
    {
        private readonly Func<Stream> _streamFunc;
        public string ContentType { get; }

        public FileResult(string contentType, Func<Stream> streamFunc)
        {
            ContentType = contentType;
            _streamFunc = streamFunc;
        }

        public ValueTask<Stream> OpenStreamAsync() => new ValueTask<Stream>(_streamFunc.Invoke());
    }
}
