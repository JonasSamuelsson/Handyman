using System;
using System.Globalization;

namespace Handyman.Extensions;

public static class Configuration
{
    public static Func<DateTimeOffset> Now;
    public static Func<IFormatProvider> FormatProvider;
    public static Func<StringComparison> StringComparison;

    static Configuration()
    {
        Reset();
    }

    public static void Reset()
    {
        Now = () => DateTimeOffset.UtcNow;
        FormatProvider = () => CultureInfo.CurrentCulture;
        StringComparison = () => System.StringComparison.CurrentCulture;
    }
}