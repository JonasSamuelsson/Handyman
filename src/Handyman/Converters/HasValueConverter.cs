using System;
using System.Globalization;

namespace Handyman.Converters
{
    public class HasValueConverter : ConverterBase<object, bool>
    {
        public override bool Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }
    }
}