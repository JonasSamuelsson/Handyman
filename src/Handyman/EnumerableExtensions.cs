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
    }
}