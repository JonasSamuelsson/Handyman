using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.AspNetCore
{
    public static class ETagConverter
    {
        private const NumberStyles NumberStyles = System.Globalization.NumberStyles.AllowHexSpecifier;
        private static readonly Regex Regex = new Regex(@"^W/""[0-9a-fA-F]+""$", RegexOptions.Compiled);

        public static string FromSqlServerRowVersion(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException();

            var strings = bytes
                .SkipWhile(x => x == 0)
                .Select(x => x.ToString("x2"));

            return $"W/\"{string.Join("", strings)}\"";
        }

        public static byte[] ToSqlServerRowVersion(string eTag)
        {
            if (eTag == null)
                throw new ArgumentNullException();

            if (eTag.Length % 2 != 0 || eTag.Length > 20 || !Regex.IsMatch(eTag))
                throw new FormatException();

            var bytes = new byte[8];

            var byteIndex = 8 - ((eTag.Length - 4) / 2);
            var stringIndex = 3;

            while (byteIndex < 8)
            {
                var s = eTag.Substring(stringIndex, 2);

                if (!byte.TryParse(s, NumberStyles, null, out var @byte))
                    throw new FormatException();

                bytes[byteIndex] = @byte;

                byteIndex += 1;
                stringIndex += 2;
            }

            return bytes;
        }
    }
}