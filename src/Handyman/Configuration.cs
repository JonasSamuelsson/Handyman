using System;
using System.Globalization;

namespace Handyman
{
    public static class Configuration
    {
        public static Func<DateTimeOffset> Now = () => DateTimeOffset.Now;
        public static Func<IFormatProvider> FormatProvider = () => CultureInfo.CurrentCulture;
    }
}