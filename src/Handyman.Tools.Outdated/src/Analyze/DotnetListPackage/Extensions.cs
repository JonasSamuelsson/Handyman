using System;
using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public static class Extensions
    {
        public static bool TryFind<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, out T value)
        {
            var found = false;
            value = default;

            foreach (var item in enumerable)
            {
                if (!predicate(item))
                    continue;

                if (found)
                    throw new InvalidOperationException();

                found = true;
                value = item;
            }

            return found;
        }

        public static T GetOrAdd<T>(this ICollection<T> collection, Func<T, bool> predicate, Func<T> factory)
        {
            if (collection.TryFind(predicate, out var result) == false)
            {
                result = factory();
                collection.Add(result);
            }

            return result;
        }
    }
}