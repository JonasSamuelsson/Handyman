using System;

namespace Handyman.AspNetCore.ApiVersioning.SemVer
{
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
            throw new NotImplementedException();
        }

        public bool IsMatch(IApiVersion other)
        {
            throw new NotImplementedException();
        }
    }
}