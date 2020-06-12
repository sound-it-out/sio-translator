using System;
using System.Collections.Generic;
using System.Linq;

namespace SIO.Translator.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> ChunkWithDelimeters(this string source, int maxChar, params char[] delimeters)
        {
            var sourceSpan = source.AsSpan();
            var items = new List<string>();

            var pointer = 0;
            var lazyPointer = 0;

            while (pointer <= sourceSpan.Length)
            {
                bool foundMatch = false;

                pointer = Math.Min(lazyPointer + maxChar, sourceSpan.Length);

                if (pointer == sourceSpan.Length)
                {
                    items.Add(sourceSpan.Slice(lazyPointer, pointer - lazyPointer).TrimStart().ToString());
                    break;
                }

                for (var j = pointer; j >= lazyPointer; j--)
                {
                    var tempChar = sourceSpan[j];

                    if (delimeters.Contains(tempChar))
                    {
                        foundMatch = true;
                        items.Add(sourceSpan.Slice(lazyPointer, j + 1 - lazyPointer).TrimStart().ToString());
                        lazyPointer = j + 1;
                    }
                }

                if (!foundMatch)
                {
                    items.Add(sourceSpan.Slice(lazyPointer, pointer - lazyPointer).Trim().ToString());
                    lazyPointer = pointer;
                }
            }

            return items;
        }
    }
}
