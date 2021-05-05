using System;
using System.Diagnostics;

namespace Handyman.AspNetCore.ApiVersioning.Internals.MajorMinorPreReleaseVersionScheme
{
    [DebuggerDisplay("major/minor/pre-release: {Text}")]
    internal class MajorMinorPreReleaseApiVersion : IApiVersion, IComparable
    {
        public string Text { get; }

        internal MajorMinorPreReleaseApiVersion(string version)
        {
            Text = version;
        }

        public bool IsMatch(IApiVersion other)
        {
            return other is MajorMinorPreReleaseApiVersion o && Text.Equals(o.Text, StringComparison.OrdinalIgnoreCase);
        }

        public int CompareTo(object obj)
        {
            var xSegments = MajorMinorPreReleaseApiVersionParser.GetSegments(this);
            var ySegments = MajorMinorPreReleaseApiVersionParser.GetSegments((MajorMinorPreReleaseApiVersion)obj);

            var i = 0;

            while (true)
            {
                if (i >= xSegments.Count || i >= ySegments.Count)
                {
                    return xSegments.Count.CompareTo(ySegments.Count);
                }

                var x = xSegments[i];
                var y = ySegments[i];

                x = x.PadLeft(y.Length, '0');
                y = y.PadLeft(x.Length, '0');

                var compare = string.Compare(x, y, StringComparison.OrdinalIgnoreCase);

                if (compare != 0)
                {
                    return compare;
                }

                i++;
            }
        }
    }
}