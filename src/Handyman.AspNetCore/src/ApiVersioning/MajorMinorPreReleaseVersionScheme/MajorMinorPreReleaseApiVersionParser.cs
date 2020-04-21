using System.Text.RegularExpressions;

namespace Handyman.AspNetCore.ApiVersioning.MajorMinorPreReleaseVersionScheme
{
    internal class MajorMinorPreReleaseApiVersionParser : IApiVersionParser
    {
        private static readonly Regex Regex = new Regex(@"^\d+(\.\d+)?(-[.0-9a-z]+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryParse(string candidate, out IApiVersion version)
        {
            if (string.IsNullOrWhiteSpace(candidate) || !Regex.IsMatch(candidate))
            {
                version = null;
                return false;
            }

            version = new MajorMinorPreReleaseApiVersion(candidate);
            return true;
        }
    }
}