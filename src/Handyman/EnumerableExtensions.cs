using System;
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

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || enumerable.IsEmpty();
        }

        public static IEnumerable<T> IfNull<T>(this IEnumerable<T> source, IEnumerable<T> @default)
        {
            return source ?? @default;
        }

        public static IEnumerable<T> IfNull<T>(this IEnumerable<T> source, Func<IEnumerable<T>> factory)
        {
            return source ?? factory();
        }

        public static IEnumerable<T> IfEmpty<T>(this IEnumerable<T> source, IEnumerable<T> @default)
        {
            if (source == null) throw new ArgumentNullException("source");
            // ReSharper disable once PossibleMultipleEnumeration
            return source.IfEmpty(() => @default);
        }

        public static IEnumerable<T> IfEmpty<T>(this IEnumerable<T> source, Func<IEnumerable<T>> factory)
        {
            if (source == null) throw new ArgumentNullException("source");
            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    do
                    {
                        yield return enumerator.Current;
                    } while (enumerator.MoveNext());
                }
                else
                {
                    foreach (var element in factory())
                        yield return element;
                }
            }
        }

        public static IEnumerable<T> IfNullOrEmpty<T>(this IEnumerable<T> source, IEnumerable<T> @default)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            return source.IfNullOrEmpty(() => @default);
        }

        public static IEnumerable<T> IfNullOrEmpty<T>(this IEnumerable<T> source, Func<IEnumerable<T>> factory)
        {
            return source == null ? factory() : source.IfEmpty(factory);
        }

        public static void Enumerate<T>(this IEnumerable<T> source)
        {
            // ReSharper disable once EmptyEmbeddedStatement
            // ReSharper disable once UnusedVariable
            foreach (var element in source) ;
        }
    }
}