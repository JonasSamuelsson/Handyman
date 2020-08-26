using System.Collections.Generic;
using System.Threading;

namespace Handyman.DuckTyping
{
    internal class ReadOptimizedLookup<TKey, TValue>
    {
        private readonly object _lock = new object();
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        internal TValue this[TKey key] => _dictionary[key];

        internal void Add(TKey key, TValue value)
        {
            lock (_lock)
            {
                var dictionary = new Dictionary<TKey, TValue>(_dictionary) { { key, value } };
                Interlocked.Exchange(ref _dictionary, dictionary);
            }
        }

        internal bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
    }
}