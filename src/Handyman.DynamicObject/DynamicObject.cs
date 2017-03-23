using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Handyman.DynamicObject
{
    public class DynamicObject
    {
        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public void Set(string key, object value)
        {
            if (value == null || value is string || value.GetType().GetTypeInfo().IsValueType)
            {
                _dictionary[key] = value;
                return;
            }

            if (value is IEnumerable)
            {
                _dictionary[key] = ((IEnumerable)value).OfType<object>().ToList();
                return;
            }

            throw new NotImplementedException();
        }

        public string Value(string key)
        {
            return Value<string>(key);
        }

        public T Value<T>(string key)
        {
            var value = _dictionary[key];
            return ConvertTo<T>(value);
        }

        public DynamicList<string> Values(string key)
        {
            var item = _dictionary[key];
            var list = (List<object>)item;
            return new DynamicList<string>(list);
        }

        public DynamicList<T> Values<T>(string key)
        {
            var item = _dictionary[key];
            var list = (List<object>)item;
            return new DynamicList<T>(list);
        }

        internal static T ConvertTo<T>(object o)
        {
            return (T)Convert.ChangeType(o, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}
