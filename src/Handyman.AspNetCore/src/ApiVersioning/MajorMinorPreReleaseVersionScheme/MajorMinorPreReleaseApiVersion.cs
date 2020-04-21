using System;
using System.Diagnostics;

namespace Handyman.AspNetCore.ApiVersioning.MajorMinorPreReleaseVersionScheme
{
    [DebuggerDisplay("major/minor/pre-release: {Text}")]
    internal class MajorMinorPreReleaseApiVersion : IApiVersion
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
    }
}