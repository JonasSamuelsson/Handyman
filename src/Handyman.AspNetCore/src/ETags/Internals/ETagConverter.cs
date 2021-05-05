using System;
using System.Linq;

namespace Handyman.AspNetCore.ETags.Internals
{
    internal class ETagConverter : IETagConverter
    {
        public string FromByteArray(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException();
            if (bytes.Length == 0) throw new ArgumentException();

            var strings = bytes.Select(x => x.ToString("x2"));

            return $"W/\"{string.Join("", strings)}\"";
        }
    }
}