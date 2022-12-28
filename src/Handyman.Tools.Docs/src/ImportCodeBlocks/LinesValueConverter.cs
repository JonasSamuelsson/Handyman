using Handyman.Tools.Docs.Shared;
using System;
using System.Linq;

namespace Handyman.Tools.Docs.ImportCodeBlocks;

public class LinesValueConverter : IValueConverter
{
    public bool TryConvert(string s, Type targetType, out object value)
    {
        value = null;

        if (int.TryParse(s, out var from))
        {
            if (from < 1)
            {
                throw new Exception($"Invalid format '{s}', value can't be less than 1.");
                return false;
            }

            value = new Lines
            {
                Count = 1,
                FromNumber = from
            };

            return true;
        }

        if (TryParse(s, '-', out from, out var to))
        {
            if (from < 1)
            {
                throw new Exception($"Invalid format '{s}', from can't be less than 1.");
            }

            if (to < @from)
            {
                throw new Exception($"Invalid format '{s}', from can't greater than to.");
            }

            value = new Lines
            {
                Count = (to - from) + 1,
                FromNumber = from
            };

            return true;
        }

        if (TryParse(s, '+', out from, out var count))
        {
            if (from < 1)
            {
                throw new Exception($"Invalid format '{s}', from can't be less than 1.");
                return false;
            }

            if (count < 1)
            {
                throw new Exception($"Invalid format '{s}', count can't be less than 1.");
                return false;
            }

            value = new Lines
            {
                Count = count + 1,
                FromNumber = from
            };
            return true;
        }

        throw new Exception($"Invalid format '{s}'.");
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