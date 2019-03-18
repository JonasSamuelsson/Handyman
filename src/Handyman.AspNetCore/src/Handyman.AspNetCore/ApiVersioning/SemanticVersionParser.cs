using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.AspNetCore.ApiVersioning
{
    internal static class SemanticVersionParser
    {
        private const string Pattern = @"^(?<major>\d+)(\.(?<minor>\d+))?(-(?<preRelease>[a-z0-9]+(\.[a-z0-9]+)*))?$";
        private static readonly ConcurrentDictionary<int, ParserResult> Cache = new ConcurrentDictionary<int, ParserResult>();

        internal static ParserResult Parse(StringValues strings)
        {
            var hash = 0;

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < strings.Count; i++)
            {
                hash ^= strings[i].GetHashCode();
            }

            return Cache.GetOrAdd(hash, _ => DoParse(strings));
        }

        private static ParserResult DoParse(StringValues strings)
        {
            var declaredVersions = new List<ParserResult.Version>(strings.Count);

            for (var i = 0; i < strings.Count; i++)
            {
                var s = strings[i];

                if (!TryParse(s, out var semanticVersion))
                    throw new FormatException($"Invalid semantic version '{s}'.");

                declaredVersions.Add(new ParserResult.Version
                {
                    SemanticVersion = semanticVersion,
                    String = s
                });
            }

            declaredVersions.Sort((x, y) => SemanticVersionComparer.Default.Compare(x.SemanticVersion, y.SemanticVersion));

            var supportedVersions = declaredVersions
                .Select(x => x.SemanticVersion)
                .GroupBy(sv => sv.Major)
                .Select(g => g.Last())
                .ToArray();

            var supportedVersionsString = string.Join(", ", supportedVersions.Select(x => x.ToString()));

            return new ParserResult
            {
                DeclaredVersions = declaredVersions.ToArray(),
                ValidationError = $"Invalid api version, supported semantic versions: {supportedVersionsString}."
            };
        }

        internal static bool TryParse(string s, out SemanticVersion semanticVersion)
        {
            semanticVersion = null;

            var match = Regex.Match(s, Pattern, RegexOptions.IgnoreCase);

            if (!match.Success)
                return false;

            var major = match.Groups["major"];
            var minor = match.Groups["minor"];

            semanticVersion = new SemanticVersion
            {
                Major = long.Parse(major.Value, NumberStyles.Integer, CultureInfo.InvariantCulture),
                Minor = minor.Success ? long.Parse(minor.Value, NumberStyles.Integer, CultureInfo.InvariantCulture) : 0,
                PreRelease = match.Groups["preRelease"]?.Value
            };

            return true;
        }
    }
}