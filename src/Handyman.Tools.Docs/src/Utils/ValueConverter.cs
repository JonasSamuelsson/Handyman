namespace Handyman.Tools.Docs.Utils
{
    public abstract class ValueConverter<TValue> : IValueConverter
    {
        bool IValueConverter.TryConvertFromString(string s, ILogger logger, out object value)
        {
            if (TryConvertFromString(s, logger, out var a))
            {
                value = a;
                return true;
            }

            value = null;
            return false;
        }

        public abstract bool TryConvertFromString(string s, ILogger logger, out TValue value);

        string IValueConverter.ConvertToString(object value)
        {
            return ConvertToString((TValue)value);
        }

        public abstract string ConvertToString(TValue value);
    }
}