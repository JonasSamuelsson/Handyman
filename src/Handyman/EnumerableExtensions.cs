using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Handyman
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }

        public static IEnumerable<T> ForEachYield<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
                yield return item;
            }
        }

        public static bool IsEmpty(this IEnumerable enumerable)
        {
            return !enumerable.Cast<object>().Any();
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] enumerable)
        {
            return source.Append(enumerable.AsEnumerable());
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, IEnumerable<T> enumerable)
        {
            foreach (var element in source) yield return element;
            foreach (var element in enumerable) yield return element;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, params T[] enumerable)
        {
            return source.Prepend(enumerable.AsEnumerable());
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, IEnumerable<T> enumerable)
        {
            return enumerable.Append(source);
        }
    }
}