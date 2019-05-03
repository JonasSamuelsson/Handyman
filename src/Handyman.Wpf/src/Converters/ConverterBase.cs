using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Handyman.Wpf.Converters
{
    public abstract class ConverterBase : MarkupExtension, IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return null;
        }
    }

    public abstract class ConverterBase<TIn, TOut> : MarkupExtension, IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((TIn)value, targetType, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((TOut)value, targetType, parameter, culture);
        }

        public abstract TOut Convert(TIn value, Type targetType, object parameter, CultureInfo culture);

        public virtual TIn ConvertBack(TOut value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return null;
        }
    }
}