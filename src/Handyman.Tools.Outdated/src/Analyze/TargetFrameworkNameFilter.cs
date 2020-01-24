using System;
using System.Text.RegularExpressions;

namespace Handyman.Tools.Outdated.Analyze
{
    public class TargetFrameworkNameFilter
    {
        private readonly string _pattern;

        private TargetFrameworkNameFilter(string pattern)
        {
            _pattern = pattern;
        }

        public bool IsMatch(string s)
        {
            return Regex.IsMatch(s, _pattern, RegexOptions.IgnoreCase);
        }

        public static TargetFrameworkNameFilter Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ApplicationException("Target framework can't be empty.");

            return new TargetFrameworkNameFilter($"^{Regex.Escape(s).Replace(@"\*", ".*")}$");
        }
    }
}