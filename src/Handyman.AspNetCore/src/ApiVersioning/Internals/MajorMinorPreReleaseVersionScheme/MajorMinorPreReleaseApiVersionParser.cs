using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.AspNetCore.ApiVersioning.Internals.MajorMinorPreReleaseVersionScheme
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

        internal static IReadOnlyList<string> GetSegments(MajorMinorPreReleaseApiVersion apiVersion)
        {
            var segments = new List<string>();

            var sections = apiVersion.Text.Split('-');

            var parts = sections[0].Split('.');

            segments.Add(parts[0]);
            segments.Add(parts.ElementAtOrDefault(1) ?? "0");

            if (sections.Length != 1)
            {
                segments.AddRange(sections[1].Split('.'));
            }

            return segments;
        }
    }
}