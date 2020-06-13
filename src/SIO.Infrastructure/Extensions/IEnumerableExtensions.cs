using System;
using System.Collections.Generic;
using System.Linq;

namespace SIO.Infrastructure.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (chunkSize <= 0)
                throw new ArgumentException($"{nameof(chunkSize)} must be greater than 0", nameof(chunkSize));

            return ChunkInternal(source, chunkSize);
        }

        private static IEnumerable<IEnumerable<T>> ChunkInternal<T>(IEnumerable<T> source, int chunkSize)
        {
            var chunk = new List<T>();

            foreach (var x in source)
            {
                chunk.Add(x);
                if (chunk.Count >= chunkSize)
                {
                    yield return chunk;
                    chunk = new List<T>();
                }
            }

            if (chunk.Any())
                yield return chunk;
        }
    }
}
