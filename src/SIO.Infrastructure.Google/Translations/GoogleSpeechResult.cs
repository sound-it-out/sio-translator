using Google.Protobuf;
using SIO.Infrastructure.Translations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Google.Translations
{
    internal sealed class GoogleSpeechResult : ISpeechResult
    {
        private readonly List<KeyValuePair<int, ByteString>> _bytes;

        public GoogleSpeechResult()
        {
            _bytes = new List<KeyValuePair<int, ByteString>>();
        }

        internal void DigestBytes(int index, ByteString bytes)
        {
            _bytes.Add(new KeyValuePair<int, ByteString>(index, bytes));
        }

        public async ValueTask<Stream> OpenStreamAsync()
        {
            return new MemoryStream(Combine(_bytes.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value.ToByteArray()).ToArray()));
        }

        private byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

    }
}
