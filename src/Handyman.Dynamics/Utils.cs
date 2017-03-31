using System;
using System.Globalization;
using System.Reflection;

namespace Handyman.Dynamics
{
    internal static class Utils
    {
        public static bool IsPrimitive(object o)
        {
            return IsPrimitive(o.GetType());
        }

        public static bool IsPrimitive(Type type)
        {
            return type == typeof(string) || type.GetTypeInfo().IsValueType;
        }

        public static T ConvertTo<T>(object o)
        {
            return (T)Convert.ChangeType(o, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}