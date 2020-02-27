using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.AspNetCore.ETags
{
    internal class ETagConverter : IETagConverter
    {
        private const NumberStyles NumberStyles = System.Globalization.NumberStyles.AllowHexSpecifier;
        private static readonly Regex Regex = new Regex(@"^W/""[0-9a-fA-F]+""$", RegexOptions.Compiled);

        public string FromByteArray(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException();
            if (bytes.Length == 0) throw new ArgumentException();

            var strings = bytes.Select(x => x.ToString("x2"));

            return $"W/\"{string.Join("", strings)}\"";
        }

        public byte[] ToByteArray(string eTag)
        {
            if (eTag == null) throw new ArgumentNullException();
            if (eTag.Length % 2 != 0 || eTag.Length > 100 || !Regex.IsMatch(eTag)) throw new FormatException();

            var count = (eTag.Length - 4) / 2;
            var bytes = new byte[count];

            for (var i = 0; i < count; i++)
            {
                var s = eTag.Substring(3 + (i * 2), 2);

                if (!byte.TryParse(s, NumberStyles, NumberFormatInfo.InvariantInfo, out var @byte))
                    throw new FormatException();

                bytes[i] = @byte;
            }

            return bytes;
        }
    }
}