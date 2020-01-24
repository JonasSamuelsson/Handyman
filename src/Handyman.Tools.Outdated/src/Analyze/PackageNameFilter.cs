using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.Tools.Outdated.Analyze
{
    public class PackageNameFilter
    {
        private readonly IEnumerable<string> _patterns;

        public PackageNameFilter(IEnumerable<string> patterns)
        {
            _patterns = patterns;
        }

        public bool IsMatch(string s)
        {
            return _patterns.Any(x => Regex.IsMatch(s, x, RegexOptions.IgnoreCase));
        }

        public static PackageNameFilter Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ApplicationException("Package name can't be empty.");

            var wildcards = s.Count(x => x == '*');

            if (wildcards == 0)
                return new PackageNameFilter(Escape(s));

            if (wildcards != 1)
                throw new ApplicationException("Package name can't use multiple wildcards.");

            if (!s.EndsWith("*"))
                throw new ApplicationException("Package name wildcards are only supported at the end.");

            var patterns = s.Contains(".*")
                ? Escape(s.Replace(".*", ""), s)
                : Escape(s);

            return new PackageNameFilter(patterns);
        }

        private static IEnumerable<string> Escape(params string[] strings)
        {
            return strings
                .Select(s =>
                {
                    var escape = Regex.Escape(s);
                    var replace = escape.Replace(@"\*", ".*");
                    return $"^{replace}$";
                })
                .ToList();
        }
    }
}