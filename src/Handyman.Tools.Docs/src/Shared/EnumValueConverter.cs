using System;

namespace Handyman.Tools.Docs.Shared;

public class EnumValueConverter : IValueConverter
{
    public bool TryConvert(string s, Type targetType, out object value)
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