using System.Diagnostics;

namespace Handyman.AspNetCore.ApiVersioning.Schemes.MajorMinorPreRelease
{
    [DebuggerDisplay("major/minor/pre-release: {Text}")]
    internal class MajorMinorPreReleaseApiVersion : IApiVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public string PreReleaseLabel { get; set; }

        public bool IsPreRelease => !string.IsNullOrWhiteSpace(PreReleaseLabel);

        public string Text => IsPreRelease
            ? $"{Major}.{Minor}-{PreReleaseLabel}"
            : $"{Major}.{Minor}";

        internal MajorMinorPreReleaseApiVersion(int major, int minor, string preReleaseLabel)
        {
            Major = major;
            Minor = minor;
            PreReleaseLabel = (preReleaseLabel ?? string.Empty).Trim();
        }

        public bool IsMatch(IApiVersion other)
        {
            return other is MajorMinorPreReleaseApiVersion o && Text == o.Text;
        }
    }
}