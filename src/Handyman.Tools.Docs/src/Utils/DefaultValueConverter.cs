using System;

namespace Handyman.Tools.Docs.Utils
{
    public class DefaultValueConverter<T> : ValueConverter<T>
    {
        public override bool TryConvertFromString(string s, ILogger logger, out T value)
        {
            value = (T)Convert.ChangeType(s, typeof(T));
            return true;
        }

        public override string ConvertToString(T value)
        {
            return (string)Convert.ChangeType(value, typeof(string));
        }
    }
}