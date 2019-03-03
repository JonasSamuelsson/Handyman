using System;
using System.Globalization;
using System.Windows;

namespace Handyman.Wpf.Converters
{
    public class Bool2VisibilityConverter : ConverterBase<bool, Visibility>
    {
        public Bool2VisibilityConverter()
        {
            False = Visibility.Collapsed;
            True = Visibility.Visible;
        }

        public Visibility False { get; set; }
        public Visibility True { get; set; }

        public override Visibility Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ? True : False;
        }

        public override bool ConvertBack(Visibility value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == False) return false;
            if (value == True) return true;
            throw new InvalidOperationException();
        }
    }
}