using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Handyman.Converters
{
    public class CompoundConverter : ConverterBase
    {
        private readonly IEnumerable<IValueConverter> _converters;

        public CompoundConverter(params IValueConverter[] converters)
        {
            _converters = converters.ToList();
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converters
                .Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converters
                .Reverse()
                .Aggregate(value, (current, converter) => converter.ConvertBack(current, targetType, parameter, culture));
        }
    }
}