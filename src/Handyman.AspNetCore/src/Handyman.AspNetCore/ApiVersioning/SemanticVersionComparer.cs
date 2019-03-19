using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Handyman.AspNetCore.ApiVersioning
{
    internal class SemanticVersionComparer : IComparer<SemanticVersion>
    {
        private static readonly string[] EmptySegments = new string[] { };
        public static IComparer<SemanticVersion> Default = new SemanticVersionComparer();

        public int Compare(SemanticVersion x, SemanticVersion y)
        {
            var major = x.Major.CompareTo(y.Major);

            if (major != 0)
                return major;

            var minor = x.Minor.CompareTo(y.Minor);

            if (minor != 0)
                return minor;

            if (x.PreRelease == y.PreRelease)
                return 0;

            var xSegments = x.PreRelease.Length == 0 ? EmptySegments : x.PreRelease.Split(new[] { "." }, StringSplitOptions.None);
            var ySegments = y.PreRelease.Length == 0 ? EmptySegments : y.PreRelease.Split(new[] { "." }, StringSplitOptions.None);

            if (xSegments.Length == 0)
                return 1;

            if (ySegments.Length == 0)
                return -1;

            for (var i = 0; ; i++)
            {
                if (i == xSegments.Length)
                    return -1;

                if (i == ySegments.Length)
                    return 1;

                var xSegment = xSegments[i];
                var ySegment = ySegments[i];

                var xSegmentIsNumeric = xSegment.All(char.IsDigit);
                var ySegmentIsNumeric = ySegment.All(char.IsDigit);

                if (xSegmentIsNumeric && ySegmentIsNumeric)
                {
                    var xNumber = long.Parse(xSegment, NumberStyles.None);
                    var yNumber = long.Parse(ySegment, NumberStyles.None);

                    var comparison = xNumber.CompareTo(yNumber);

                    if (comparison == 0)
                        continue;

                    return comparison;
                }

                if (!xSegmentIsNumeric && !ySegmentIsNumeric)
                {
                    var comparison = string.Compare(xSegment, ySegment, StringComparison.OrdinalIgnoreCase);

                    if (comparison == 0)
                        continue;

                    return comparison;
                }

                return ySegmentIsNumeric.CompareTo(xSegmentIsNumeric);
            }
        }
    }
}