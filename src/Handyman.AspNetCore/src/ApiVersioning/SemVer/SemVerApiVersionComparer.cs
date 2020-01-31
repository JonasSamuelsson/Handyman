using System;

namespace Handyman.AspNetCore.ApiVersioning.SemVer
{
    internal static class SemVerApiVersionComparer
    {
        public static int Compare(SemVerApiVersion x, SemVerApiVersion y)
        {
            var diff = x.Major.CompareTo(y.Major);

            if (diff != 0)
                return diff;

            diff = x.Minor.CompareTo(y.Minor);

            if (diff != 0)
                return diff;

            diff = y.IsPreRelease.CompareTo(x.IsPreRelease);

            if (diff != 0)
                return diff;

            var xLength = x.PreReleaseLabels.Length;
            var yLength = y.PreReleaseLabels.Length;
            var length = Math.Max(xLength, yLength);

            for (var i = 0; i < length; i++)
            {
                if (i == xLength)
                    return -1;

                if (i == yLength)
                    return 1;

                var xLabel = x.PreReleaseLabels[i];
                var yLabel = y.PreReleaseLabels[i];

                diff = xLabel is int xInt && yLabel is int yInt
                    ? xInt.CompareTo(yInt)
                    : StringComparer.OrdinalIgnoreCase.Compare(xLabel, yLabel);

                if (diff == 0)
                    continue;

                return diff;
            }

            return 0;
        }
    }
}