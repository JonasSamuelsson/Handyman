using Handyman.Tools.Docs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Docs.BuildTablesOfContents;

public class LevelsValueConverter : IValueConverter
{
    public bool TryConvert(string s, Type targetType, out object value)
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
                    throw new Exception("todo");
                }

                value = new TableOfContentLevels
                {
                    Current = true,
                    CurrentAdditionalLevels = additionalLevels,
                    ExplicitLevels = ArraySegment<int>.Empty
                };

                return true;
            }

            throw new Exception("todo");
        }

        var levels = Parse(s);

        if (!levels.Any())
        {
            throw new Exception("todo");
        }

        var min = levels.Min();
        var max = levels.Max();

        if (levels.Count != 1 && min >= max)
        {
            throw new Exception("todo");
        }

        if (min < 1)
        {
            throw new Exception("todo");
        }

        if (6 < max)
        {
            throw new Exception("todo");
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
            throw new Exception("todo");
        }

        if (int.TryParse(s, out var level))
        {
            return new[] { level };
        }

        var strings = s.Split('-');

        if (strings.Length != 2)
        {
            throw new Exception("todo");
        }

        if (int.TryParse(strings[0], out var from) && int.TryParse(strings[1], out var to))
        {
            return Enumerable.Range(1, 6)
                .Where(i => from <= i)
                .Where(i => i <= to)
                .ToList();
        }

        throw new Exception("todo");
    }
}