using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Extensions
{
    public static class LookupExtensions
    {
        public static bool TryGetElements<TKey, TElement>(this ILookup<TKey, TElement> lookup, TKey key, out IEnumerable<TElement> result)
        {
            var contains = lookup.Contains(key);
            result = contains ? lookup[key] : null;
            return contains;
        }

        public static IEnumerable<TElement> GetElementsOrEmpty<TKey, TElement>(this ILookup<TKey, TElement> lookup, TKey key)
        {
            return lookup.GetElementsOrDefault(key, new TElement[] { });
        }

        public static IEnumerable<TElement> GetElementsOrDefault<TKey, TElement>(this ILookup<TKey, TElement> lookup, TKey key)
        {
            return lookup.GetElementsOrDefault(key, (IEnumerable<TElement>)null);
        }

        public static IEnumerable<TElement> GetElementsOrDefault<TKey, TElement>(this ILookup<TKey, TElement> lookup, TKey key, IEnumerable<TElement> @default)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            return lookup.GetElementsOrDefault(key, () => @default);
        }

        public static IEnumerable<TElement> GetElementsOrDefault<TKey, TElement>(this ILookup<TKey, TElement> lookup, TKey key, Func<IEnumerable<TElement>> factory)
        {
            IEnumerable<TElement> result;
            return lookup.TryGetElements(key, out result) ? result : factory();
        }
    }
}