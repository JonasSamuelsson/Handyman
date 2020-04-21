using System.Text.RegularExpressions;

namespace Handyman.AspNetCore.ApiVersioning.MajorMinorPreReleaseVersionScheme
{
    internal class MajorMinorPreReleaseApiVersionParser : IApiVersionParser
    {
        private static readonly Regex Regex = new Regex(@"^\d+(\.\d+)?(-[.0-9a-z]+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryParse(string candidate, out IApiVersion version)
        {
            if (candidate == null || !Regex.IsMatch(candidate))
            {
                version = null;
                return false;
            }

            var dotIndex = candidate.IndexOf('.');

            if (dotIndex == -1)
            {
                var dashIndex = candidate.IndexOf('-');

                if (dashIndex == -1)
                {
                    candidate += ".0";
                }
                else
                {
                    candidate = candidate.Insert(dashIndex, ".0");
                }
            }

            version = new MajorMinorPreReleaseApiVersion(candidate);
            return true;
        }
    }
}