using System;
using System.Globalization;

namespace Handyman.AspNetCore.ApiVersioning.Schemes.MajorMinorPreRelease
{
    internal class MajorMinorPreReleaseApiVersionParser : IApiVersionParser
    {
        public bool TryParse(string candidate, out IApiVersion version)
        {
            version = null;

            if (string.IsNullOrWhiteSpace(candidate))
                return false;

            var strings = candidate.Split(new[] { '-' }, 2, StringSplitOptions.None);

            var numbers = strings[0].Split(new[] { '.' }, 2, StringSplitOptions.None);

            if (!TryParseInt(numbers[0], out var major))
                return false;

            var minor = 0;
            if (numbers.Length == 2 && !TryParseInt(numbers[1], out minor))
                return false;

            var preRelease = string.Empty;

            if (strings.Length == 2)
            {
                preRelease = strings[1];

                if (string.IsNullOrWhiteSpace(preRelease))
                    return false;
            }

            version = new MajorMinorPreReleaseApiVersion(major, minor, preRelease);
            return true;
        }

        private static bool TryParseInt(string s, out int i)
        {
            return int.TryParse(s, NumberStyles.None, NumberFormatInfo.InvariantInfo, out i) && i >= 0;
        }
    }
}