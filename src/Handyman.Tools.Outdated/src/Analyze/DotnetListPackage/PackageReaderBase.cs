using Handyman.Tools.Outdated.Model;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public abstract class PackageReaderBase
    {
        public void Read(OutputEnumerator enumerator, TargetFramework targetFramework)
        {
            while (true)
            {
                var headers = Split(enumerator.Current);

                if (headers.Any() == false || TryParseHeaders(headers, out var isTransitive, out var currentVersionIndex) == false)
                    return;

                enumerator.MoveNext();

                var strings = Split(enumerator.Current);

                while (strings.Any() && TryParseName(strings[0], out var name))
                {
                    var package = targetFramework.Packages.GetOrAdd(x => x.Name == name, factory: () =>
                    {
                        var pkg = new Package
                        {
                            IsTransitive = isTransitive,
                            Name = name,
                        };

                        var currentVersionCandidate = strings[currentVersionIndex];

                        if (TryParseVersion(currentVersionCandidate, out var currentVersion, out var info, out var deprecated))
                        {
                            pkg.CurrentVersion = currentVersion;
                            pkg.Deprecation.IsDeprecated = deprecated;
                            pkg.Info = info;
                        }
                        else
                        {
                            pkg.Info = currentVersionCandidate;
                        }

                        return pkg;
                    });

                    Read(strings.Skip(currentVersionIndex + 1).ToArray(), package);

                    enumerator.MoveNext();
                    strings = Split(enumerator.Current);
                }
            }
        }

        private static string[] Split(string line)
        {
            return line.Split("   ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
        }

        private static bool TryParseName(string s, out string name)
        {
            if (s.StartsWith("> ") == false)
            {
                name = null;
                return false;
            }

            name = s.Substring(2);
            return true;
        }

        private static bool TryParseHeaders(string[] headers, out bool isTransitive, out int currentVersionIndex)
        {
            isTransitive = false;
            currentVersionIndex = 0;

            var first = headers[0];

            if (first.StartsWith("Top-level Package"))
            {
                isTransitive = false;
            }
            else if (first.StartsWith("Transitive Package"))
            {
                isTransitive = true;
            }
            else
            {
                return false;
            }

            for (var i = 0; i < headers.Length; i++)
            {
                if (headers[i] != "Resolved")
                    continue;

                currentVersionIndex = i;
                return true;
            }

            return false;
        }

        protected abstract void Read(string[] strings, Package package);

        protected static bool TryParseVersion(string s, out string version, out string info, out bool deprecated)
        {
            var match = Regex.Match(s, @"^(?<version>\d+\.\d+\.\d+\S*)( (?<info>.+))?$");

            deprecated = false;

            if (match.Success == false)
            {
                version = string.Empty;
                info = s;
                return false;
            }

            version = match.Groups["version"].Value;
            info = match.Groups["info"].Value;

            if (info == "(D)")
            {
                deprecated = true;
                info = string.Empty;
            }

            return true;
        }
    }
}