using System;
using System.Linq;

namespace Handyman.Tools.Outdated.Analyze
{
    public class PackageVersionFilter
    {
        private int _major;
        private int _minor;
        private int _patch;

        private PackageVersionFilter() { }

        public static PackageVersionFilter Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ApplicationException("Package version can't be empty.");

            var filter = new PackageVersionFilter();

            if (TryParse(s, out filter._major, out filter._minor, out filter._patch))
                return filter;

            throw new ApplicationException($"Invalid version '{s}'.");
        }

        private static bool TryParse(string s, out int major, out int minor, out int patch)
        {
            major = 0;
            minor = 0;
            patch = 0;

            var strings = s.Split('-');
            var numbers = strings[0].Split('.');

            if (numbers.Length > 3)
                return false;

            return TryParse(numbers.ElementAtOrDefault(0), out major)
                   && TryParse(numbers.ElementAtOrDefault(1), out minor)
                   && TryParse(numbers.ElementAtOrDefault(2), out patch);
        }

        private static bool TryParse(string s, out int i)
        {
            if (s == null)
            {
                i = 0;
                return true;
            }

            if (s == "*")
            {
                i = int.MaxValue;
                return true;
            }

            return int.TryParse(s, out i) && i >= 0;
        }

        public bool IsMatch(string version)
        {
            if (!TryParse(version, out var major, out var minor, out var patch))
                throw new ApplicationException($"Invalid version '{version}'.");

            if (major > _major)
                return false;

            if (major < _major)
                return true;

            if (minor > _minor)
                return false;

            if (minor < _minor)
                return true;

            return patch <= _patch;
        }
    }
}