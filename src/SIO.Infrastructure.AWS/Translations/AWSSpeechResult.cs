using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SIO.Infrastructure.Translations;

namespace SIO.Infrastructure.AWS.Translations
{
    public class AWSSpeechResult : ISpeechResult
    {
        private readonly ConcurrentBag<KeyValuePair<int, byte[]>> _bytes;

        public AWSSpeechResult()
        {
            _bytes = new ConcurrentBag<KeyValuePair<int, byte[]>>();
        }

        internal async Task DigestBytes(int index, Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                _bytes.Add(new KeyValuePair<int, byte[]>(index, ms.ToArray()));
            }            
        }

        public ValueTask<Stream> OpenStreamAsync()
        {
            return new ValueTask<Stream>(new MemoryStream(Combine(_bytes.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray())));
        }

        private byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}
