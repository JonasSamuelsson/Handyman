namespace HandymanExtensionsTests;

public class LookupExtensionsTests
{
    [Fact]
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

    [Fact]
    public void GetElementsOrDefault()
    {
        var lookup = new Lookup<int, string>();

        lookup.GetElementsOrDefault(0, new[] { "default" }).ShouldBe(new[] { "default" });
        lookup.GetElementsOrDefault(0, () => new[] { "default" }).ShouldBe(new[] { "default" });

        lookup.Add(0, "zero");

        lookup.GetElementsOrDefault(0, new[] { "default" }).ShouldBe(new[] { "zero" });
        lookup.GetElementsOrDefault(0, () => new[] { "default" }).ShouldBe(new[] { "zero" });
    }

    [Fact]
    public void GetElementsOrNull()
    {
        var lookup = new Lookup<int, string>();

        lookup.GetElementsOrNull(0).ShouldBe(null);

        lookup.Add(0, "zero");

        lookup.GetElementsOrNull(0).ShouldBe(new[] { "zero" });
    }

    [Fact]
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