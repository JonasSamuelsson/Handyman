using System;
using System.Globalization;

namespace Handyman.AspNetCore.ApiVersioning.SemVer
{
    internal class SemVerApiVersionParser : IApiVersionParser
    {
        private static readonly object[] EmptyPreReleaseLabels = new object[] { };

        internal SemVerApiVersion Parse(string candidate)
        {
            return TryParse(candidate, out var version) ? (SemVerApiVersion)version : throw new FormatException();
        }

        public bool TryParse(string candidate, out IApiVersion version)
        {
            version = null;

            // todo improve perf, reduce allocations

            var strings = candidate.Split('-');

            if (strings.Length > 2)
                return false;

            var ints = strings[0].Split('.');

            if (ints.Length > 2)
                return false;

            if (!int.TryParse(ints[0], NumberStyles.None, CultureInfo.InvariantCulture, out var major))
                return false;

            var minor = 0;

            if (ints.Length == 2)
            {
                if (!int.TryParse(ints[1], NumberStyles.None, CultureInfo.InvariantCulture, out minor))
                    return false;
            }

            if (strings.Length == 1)
            {
                version = new SemVerApiVersion(major, minor, EmptyPreReleaseLabels);
            }
            else
            {
                strings = strings[1].Split('.');
                var labels = new object[strings.Length];

                for (var i = 0; i < labels.Length; i++)
                {
                    var s = strings[i];

                    if (s == string.Empty)
                        return false;

                    if (int.TryParse(s, out var number))
                    {
                        labels[i] = number;
                    }
                    else
                    {
                        labels[i] = s;
                    }
                }

                version = new SemVerApiVersion(major, minor, labels);
            }

            return true;
        }
    }
}