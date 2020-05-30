using Handyman.Tools.Outdated.Model;
using System;
using System.Linq;

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
                        var currentVersion = strings[currentVersionIndex];

                        return new Package
                        {
                            CurrentVersion = currentVersion,
                            IsTransitive = isTransitive,
                            Name = name
                        };
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
    }
}