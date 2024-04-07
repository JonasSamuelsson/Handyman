using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Handyman.Extensions;

public static class ConcurrentDictionaryExtensions
{
    public static void AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        dictionary.AddOrUpdate(key, value, (_, _) => value);
    }

    public static TValue GetOrThrow<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
    }
}