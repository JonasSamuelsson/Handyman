using System;

namespace Handyman.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNull<T>(this T instance) where T : class
        {
            return instance == null;
        }

        public static bool IsNotNull<T>(this T instance) where T : class
        {
            return !instance.IsNull();
        }

        public static T Coalesce<T>(this T value, Func<T, bool> predicate, T @default)
        {
            return predicate(value) ? value : @default;
        }

        public static T Coalesce<T>(this T value, Func<T, bool> predicate, Func<T> factory)
        {
            return predicate(value) ? value : factory();
        }

        public static T Coalesce<T>(this T value, Func<T, bool> predicate, Func<T, T> factory)
        {
            return predicate(value) ? value : factory(value);
        }
    }
}
