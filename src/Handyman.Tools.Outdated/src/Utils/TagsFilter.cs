using Handyman.Tools.Outdated.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Outdated.Utils
{
    public static class TagsFilter
    {
        public static bool IsMatch(Project project, IReadOnlyCollection<string> tags)
        {
            if (!tags.Any())
                return true;

            var includes = tags
                .Where(x => !x.StartsWith("!"))
                .Select(x => x.ToLowerInvariant())
                .ToList();

            var excludes = tags
                .Where(x => x.StartsWith("!"))
                .Select(x => x.Substring(1))
                .Select(x => x.ToLowerInvariant())
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            if (includes.Any() && !includes.All(x => project.Config.Tags.Any(y => x.Equals(y, StringComparison.InvariantCultureIgnoreCase))))
                return false;

            if (project.Config.Tags.Any(x => excludes.Contains(x)))
                return false;

            return true;
        }
    }
}