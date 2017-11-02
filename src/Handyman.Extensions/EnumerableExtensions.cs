using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }

        public static IEnumerable<T> Visit<T>(this IEnumerable<T> collection, Action<T> action)
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

        public static void Enumerate<T>(this IEnumerable<T> source)
        {
            // ReSharper disable once EmptyEmbeddedStatement
            // ReSharper disable once UnusedVariable
            foreach (var element in source) ;
        }

        public static TimeSpan Sum<T>(this IEnumerable<T> source, Func<T, TimeSpan> selector)
        {
            return source.Select(selector).Sum();
        }

        public static TimeSpan Sum(this IEnumerable<TimeSpan> source)
        {
            return TimeSpan.FromTicks(source.Sum(x => x.Ticks));
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var random = new Random();
            var list = source.ToList();
            for (var i = list.Count; i > 1; i--)
            {
                var j = random.Next(i);
                var temp = list[j];
                list[j] = list[i - 1];
                list[i - 1] = temp;
            }
            return list;
        }

        public static ISet<T> ToSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
        {
            return source as IReadOnlyList<T> ?? source.ToList();
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
        {
            var list = source.ToReadOnlyList();
            return list.Take(list.Count - count);
        }

        public static IEnumerable<T> SkipLastWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var list = source.ToReadOnlyList();
            var i = 1;
            for (; i <= list.Count; i++)
                if (!predicate(list[list.Count - i])) break;
            return list.SkipLast(i - 1);
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
        {
            var list = source.ToReadOnlyList();
            return list.Skip(list.Count - count);
        }

        public static IEnumerable<T> TakeLastWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var list = source.ToReadOnlyList();
            var i = 1;
            for (; i <= list.Count; i++)
                if (!predicate(list[list.Count - i])) break;
            return list.TakeLast(i - 1);
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            var buffer = new List<T>(chunkSize);
            foreach (var item in source)
            {
                buffer.Add(item);
                if (buffer.Count != chunkSize) continue;
                yield return buffer;
                buffer = new List<T>(chunkSize);
            }
            if (buffer.Count != 0) yield return buffer;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var index = 0;
            foreach (var item in source) action(item, index++);
        }

        public static IEnumerable<T> Visit<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var index = 0;
            foreach (var item in source)
            {
                action(item, index++);
                yield return item;
            }
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var buffer = new List<T>();
            foreach (var item in source)
            {
                if (predicate(item) && buffer.Count != 0)
                {
                    yield return buffer;
                    buffer = new List<T>();
                }

                buffer.Add(item);
            }

            if (buffer.Count != 0)
                yield return buffer;
        }

        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            return source as IReadOnlyCollection<T> ?? source.ToList();
        }

        public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
        {
            return source as Queue<T> ?? new Queue<T>(source);
        }

        public static Stack<T> ToStack<T>(this IEnumerable<T> source)
        {
            return source as Stack<T> ?? new Stack<T>(source);
        }

        public static ConcurrentBag<T> ToConcurrentBag<T>(this IEnumerable<T> source)
        {
            return source as ConcurrentBag<T> ?? new ConcurrentBag<T>(source);
        }

        public static ConcurrentDictionary<TKey, T> ToConcurrentDictionary<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            return source.ToConcurrentDictionary(keySelector, x => x);
        }

        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TValue> valueSelector)
        {
            var dictionary = new ConcurrentDictionary<TKey, TValue>();
            foreach (var item in source)
            {
                var key = keySelector(item);
                var value = valueSelector(item);
                if (dictionary.TryAdd(key, value)) continue;
                throw new InvalidOperationException();
            }
            return dictionary;
        }

        public static ConcurrentQueue<T> ToConcurrentQueue<T>(this IEnumerable<T> source)
        {
            return source as ConcurrentQueue<T> ?? new ConcurrentQueue<T>(source);
        }

        public static ConcurrentStack<T> ToConcurrentStack<T>(this IEnumerable<T> source)
        {
            return source as ConcurrentStack<T> ?? new ConcurrentStack<T>(source);
        }

        public static T MinOrDefault<T>(this IEnumerable<T> source)
        {
            return source.MinOrDefault(default(T));
        }

        public static T MinOrDefault<T>(this IEnumerable<T> source, T @default)
        {
            return source.MinOrDefault(() => @default);
        }

        public static T MinOrDefault<T>(this IEnumerable<T> source, Func<T> factory)
        {
            return source.DefaultIfEmpty(factory()).Min();
        }

        public static TValue MinOrDefault<T, TValue>(this IEnumerable<T> source, Func<T, TValue> selector)
        {
            return source.Select(selector).MinOrDefault(default(TValue));
        }

        public static TValue MinOrDefault<T, TValue>(this IEnumerable<T> source, Func<T, TValue> selector, TValue @default)
        {
            return source.Select(selector).MinOrDefault(() => @default);
        }

        public static TValue MinOrDefault<T, TValue>(this IEnumerable<T> source, Func<T, TValue> selector, Func<TValue> factory)
        {
            return source.Select(selector).DefaultIfEmpty(factory()).Min();
        }

        public static T MaxOrDefault<T>(this IEnumerable<T> source)
        {
            return source.MaxOrDefault(default(T));
        }

        public static T MaxOrDefault<T>(this IEnumerable<T> source, T @default)
        {
            return source.MaxOrDefault(() => @default);
        }

        public static T MaxOrDefault<T>(this IEnumerable<T> source, Func<T> factory)
        {
            return source.DefaultIfEmpty(factory()).Max();
        }

        public static TValue MaxOrDefault<T, TValue>(this IEnumerable<T> source, Func<T, TValue> selector)
        {
            return source.Select(selector).MaxOrDefault(default(TValue));
        }

        public static TValue MaxOrDefault<T, TValue>(this IEnumerable<T> source, Func<T, TValue> selector, TValue @default)
        {
            return source.Select(selector).MaxOrDefault(() => @default);
        }

        public static TValue MaxOrDefault<T, TValue>(this IEnumerable<T> source, Func<T, TValue> selector, Func<TValue> factory)
        {
            return source.Select(selector).DefaultIfEmpty(factory()).Max();
        }

        public static IEnumerable<T> Visit<T>(this IEnumerable<T> source, Func<T, bool> predicate, Action<T> action)
        {
            return source.Visit((index, item) => predicate(item), (index, item) => action(item));
        }

        public static IEnumerable<T> Visit<T>(this IEnumerable<T> source, Func<int, T, bool> predicate, Action<int, T> action)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (predicate(index, item)) action(index, item);
                yield return item;
                index++;
            }
        }

        public static IEnumerable<T> GetElementsOrDefault<T>(this IEnumerable<T> source, IEnumerable<T> @default)
        {
            return source ?? @default;
        }

        public static IEnumerable<T> GetElementsOrDefault<T>(this IEnumerable<T> source, Func<IEnumerable<T>> factory)
        {
            return source ?? factory();
        }

        public static IEnumerable<T> GetElementsOrEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}