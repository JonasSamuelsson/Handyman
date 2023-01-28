using System;

namespace Handyman.Tools.Docs.Shared;

public class EnumParser : IValueParser
{
    public bool TryParse(string s, Type targetType, out object value)
    {
        if (!targetType.IsEnum)
        {
            value = null;
            return false;
        }

        value = Enum.Parse(targetType, s, ignoreCase: true);
        return true;
    }
}