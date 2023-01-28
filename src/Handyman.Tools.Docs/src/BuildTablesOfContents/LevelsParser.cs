using Handyman.Tools.Docs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Docs.BuildTablesOfContents;

public class LevelsParser : IValueParser
{
    public bool TryParse(string s, Type targetType, out object value)
    {
        if (targetType != typeof(TableOfContentLevels))
        {
            value = null;
            return false;
        }

        if (s.StartsWith("current"))
        {
            if (s == "current")
            {
                value = new TableOfContentLevels
                {
                    Current = true,
                    CurrentAdditionalLevels = 0,
                    ExplicitLevels = ArraySegment<int>.Empty
                };

                return true;
            }

            if (s.StartsWith("current+") && s.Length > 8 && int.TryParse(s.Substring(8), out var additionalLevels))
            {
                if (additionalLevels < 1 || 6 < additionalLevels)
                {
                    throw new TodoException();
                }

                value = new TableOfContentLevels
                {
                    Current = true,
                    CurrentAdditionalLevels = additionalLevels,
                    ExplicitLevels = ArraySegment<int>.Empty
                };

                return true;
            }

            throw new TodoException();
        }

        var levels = Parse(s);

        if (!levels.Any())
        {
            throw new TodoException();
        }

        var min = levels.Min();
        var max = levels.Max();

        if (levels.Count != 1 && min >= max)
        {
            throw new TodoException();
        }

        if (min < 1)
        {
            throw new TodoException();
        }

        if (6 < max)
        {
            throw new TodoException();
        }

        value = new TableOfContentLevels
        {
            Current = false,
            CurrentAdditionalLevels = 0,
            ExplicitLevels = levels
        };

        return true;
    }

    private IReadOnlyCollection<int> Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            throw new TodoException();
        }

        if (int.TryParse(s, out var level))
        {
            return new[] { level };
        }

        var strings = s.Split('-');

        if (strings.Length != 2)
        {
            throw new TodoException();
        }

        if (int.TryParse(strings[0], out var from) && int.TryParse(strings[1], out var to))
        {
            return Enumerable.Range(1, 6)
                .Where(i => from <= i)
                .Where(i => i <= to)
                .ToList();
        }

        throw new TodoException();
    }
}