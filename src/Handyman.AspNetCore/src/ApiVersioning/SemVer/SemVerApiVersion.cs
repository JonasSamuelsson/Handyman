using System;
using System.Diagnostics;

namespace Handyman.AspNetCore.ApiVersioning.SemVer
{
    [DebuggerDisplay("{Text}")]
    internal sealed class SemVerApiVersion : IApiVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public object[] PreReleaseLabels { get; set; }

        public bool IsPreRelease => PreReleaseLabels.Length != 0;

        public string Text => IsPreRelease
            ? $"{Major}.{Minor}-{string.Join(".", PreReleaseLabels)}"
            : $"{Major}.{Minor}";

        internal SemVerApiVersion(int major, int minor, object[] preReleaseLabels)
        {
            Major = major;
            Minor = minor;
            PreReleaseLabels = preReleaseLabels ?? throw new ArgumentNullException(nameof(preReleaseLabels));
        }

        public int CompareTo(IApiVersion other)
        {
            if (!(other is SemVerApiVersion o))
                throw new ArgumentException();

            return SemVerApiVersionComparer.Compare(this, o);
        }

        public bool IsMatch(IApiVersion other)
        {
            return other is SemVerApiVersion o && SemVerApiVersionComparer.IsMatch(this, o);
        }
    }
}