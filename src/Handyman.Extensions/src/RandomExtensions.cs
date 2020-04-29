using System;

namespace Handyman.Extensions
{
    public static class RandomExtensions
    {
        public static byte[] NextBytes(this Random random, int length)
        {
            var bytes = new byte[length];
            random.NextBytes(bytes);
            return bytes;
        }
    }
}