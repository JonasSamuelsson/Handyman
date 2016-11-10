using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handyman.Extensions;
using Shouldly;

namespace Handyman.Tests.Extensions
{
    public class LookupExtensionsTests
    {
        public void TryGetElements()
        {
            var lookup = new Lookup<int, string>();

            IEnumerable<string> result;
            lookup.TryGetElements(0, out result).ShouldBe(false);
            result.ShouldBe(null);

            lookup.Add(0, "zero");

            lookup.TryGetElements(0, out result).ShouldBe(true);
            result.ShouldBe(new[] { "zero" });
        }

        public void GetElementsOrDefault()
        {
            var lookup = new Lookup<int, string>();

            lookup.GetElementsOrDefault(0).ShouldBe(null);
            lookup.GetElementsOrDefault(0, new[] { "default" }).ShouldBe(new[] { "default" });
            lookup.GetElementsOrDefault(0, () => new[] { "default" }).ShouldBe(new[] { "default" });

            lookup.Add(0, "zero");

            lookup.GetElementsOrDefault(0).ShouldBe(new[] { "zero" });
            lookup.GetElementsOrDefault(0, new[] { "default" }).ShouldBe(new[] { "zero" });
            lookup.GetElementsOrDefault(0, () => new[] { "default" }).ShouldBe(new[] { "zero" });
        }

        public void GetElementsOrEmpty()
        {
            var lookup = new Lookup<int, string>();

            lookup.GetElementsOrEmpty(0).ShouldBe(new string[] { });

            lookup.Add(0, "zero");

            lookup.GetElementsOrEmpty(0).ShouldBe(new[] { "zero" });
        }

        class Lookup<TKey, TElement> : ILookup<TKey, TElement>
        {
            private readonly ICollection<KeyValuePair<TKey, TElement>> _collection = new List<KeyValuePair<TKey, TElement>>();

            private ILookup<TKey, TElement> GetLookup()
            {
                return _collection.ToLookup(x => x.Key, x => x.Value);
            }

            public void Add(TKey key, TElement value)
            {
                _collection.Add(new KeyValuePair<TKey, TElement>(key, value));
            }

            public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
            {
                return GetLookup().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool Contains(TKey key)
            {
                return GetLookup().Contains(key);
            }

            public int Count
            {
                get { return GetLookup().Count; }
            }

            public IEnumerable<TElement> this[TKey key]
            {
                get { return GetLookup()[key]; }
            }
        }
    }
}