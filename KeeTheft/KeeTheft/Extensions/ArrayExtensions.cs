using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeeTheft.Extensions
{
    static class ArrayExtensions
    {
        // Returns the first index of a sequence of bytes
        // Based on http://stackoverflow.com/questions/283456/byte-array-pattern-search
        public static int IndexOfSequence(this byte[] buffer, byte[] pattern, int startIndex)
        {
            int i = Array.IndexOf<byte>(buffer, pattern[0], startIndex);
            while (i >= 0 && i <= buffer.Length - pattern.Length)
            {
                byte[] segment = new byte[pattern.Length];
                Buffer.BlockCopy(buffer, i, segment, 0, pattern.Length);
                if (segment.SequenceEqual<byte>(pattern))
                    return i;
                i = Array.IndexOf<byte>(buffer, pattern[0], i + pattern.Length);
            }

            return -1;
        }
    }
}
