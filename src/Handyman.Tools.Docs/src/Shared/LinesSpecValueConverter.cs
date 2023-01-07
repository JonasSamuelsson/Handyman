using System;
using System.Linq;

namespace Handyman.Tools.Docs.Shared;

public class LinesSpecValueConverter : IValueConverter
{
    public bool TryConvert(string s, Type targetType, out object value)
    {
        if (targetType != typeof(LinesSpec))
        {
            value = null;
            return false;
        }

        value = new LinesSpec
        {
            Sections = s.Split(',').Select(ParseSection).ToList()
        };

        return true;
    }

    private static LinesSpec.Section ParseSection(string s)
    {
        if (int.TryParse(s, out var from))
        {
            if (from < 1)
            {
                throw new Exception($"Invalid format '{s}', value can't be less than 1.");
            }

            return CreateSection(from, from);
        }

        if (TryParse(s, '-', out from, out var to))
        {
            if (from < 1)
            {
                throw new Exception($"Invalid format '{s}', from can't be less than 1.");
            }

            if (to < from)
            {
                throw new Exception($"Invalid format '{s}', from can't greater than to.");
            }

            return CreateSection(from, to);
        }

        throw new Exception($"Invalid format '{s}'.");
    }

    private static LinesSpec.Section CreateSection(int from, int to)
    {
        return new LinesSpec.Section
        {
            FromNumber = from,
            Count = to - from + 1
        };
    }

    private static bool TryParse(string s, char separator, out int first, out int second)
    {
        first = 0;
        second = 0;

        if (s.Count(x => x == separator) != 1)
            return false;

        var index = s.IndexOf(separator);

        if (index <= 0 || index == s.Length - 1)
            return false;

        var s1 = s.Substring(0, index);
        var s2 = s.Substring(index + 1);

        return int.TryParse(s1, out first) && int.TryParse(s2, out second);
    }
}