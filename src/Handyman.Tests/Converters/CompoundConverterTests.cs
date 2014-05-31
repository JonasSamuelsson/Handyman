using Handyman.Converters;
using Shouldly;
using System;
using System.Globalization;
using System.Windows;

namespace Handyman.Tests.Converters
{
    public class CompoundConverterTests
    {
        public void ShouldConvert()
        {
            new CompoundConverter(new String2BoolConverter(), new Bool2VisibilityConverter())
                .Convert("true", null, null, null).ShouldBe(Visibility.Visible);
        }

        public void ShouldConvertBack()
        {
            new CompoundConverter(new String2BoolConverter(), new Bool2VisibilityConverter())
                .ConvertBack(Visibility.Collapsed, null, null, null).ShouldBe("False");
        }

        private class String2BoolConverter : ConverterBase<string, bool>
        {
            public override bool Convert(string value, Type targetType, object parameter, CultureInfo culture)
            {
                return bool.Parse(value);
            }

            public override string ConvertBack(bool value, Type targetType, object parameter, CultureInfo culture)
            {
                return value.ToString(culture);
            }
        }
    }
}