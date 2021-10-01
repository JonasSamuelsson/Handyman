using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator
{
    public class Enum
    {
        public Enum(params int[] ids)
            : this(ids.Select(id => new Value { Id = id }))
        { }

        public Enum(IEnumerable<int> ids)
            : this(ids.Select(id => new Value { Id = id }))
        { }

        public Enum(params long[] ids)
            : this(ids.Select(id => new Value { Id = id }))
        { }

        public Enum(IEnumerable<long> ids)
            : this(ids.Select(id => new Value { Id = id }))
        { }

        public Enum(params string[] names)
            : this(names.Select(name => new Value { Name = name }))
        { }

        public Enum(IEnumerable<string> names)
            : this(names.Select(name => new Value { Name = name }))
        { }

        public Enum(IEnumerable<KeyValuePair<int, string>> values)
            : this(values.Select(x => new Value { Id = x.Key, Name = x.Value }))
        { }

        public Enum(IEnumerable<KeyValuePair<long, string>> values)
            : this(values.Select(x => new Value { Id = x.Key, Name = x.Value }))
        { }

        private Enum(IEnumerable<Value> values)
        {
            Values = values.ToList();
        }

        public bool Flags { get; set; }
        public bool Nullable { get; set; }

        internal IReadOnlyList<Value> Values { get; }

        internal bool HasIds => Values.Any(x => x.Id != null);
        internal bool HasNames => Values.Any(x => x.Name != null);

        internal IEnumerable<long> Ids => Values.Select(x => (long)x.Id);
        internal IEnumerable<string> Names => Values.Select(x => x.Name);

        internal struct Value
        {
            public long? Id { get; set; }
            public string Name { get; set; }
        }
    }
}