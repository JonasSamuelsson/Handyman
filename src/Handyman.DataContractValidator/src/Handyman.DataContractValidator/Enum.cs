using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator
{
    public class Enum
    {
        public Enum(params long[] values)
            : this(EnumKind.Default, values)
        { }

        public Enum(EnumKind enumKind, IEnumerable<int> values)
        : this(enumKind, values.Select(x => Convert.ChangeType(x, typeof(long))).Cast<long>())
        { }

        public Enum(EnumKind enumKind, IEnumerable<long> values)
        {
            EnumKind = enumKind;
            EnumValues = values;
        }

        internal IEnumerable<long> EnumValues { get; }
        internal EnumKind EnumKind { get; }
    }

    [Flags]
    public enum EnumKind
    {
        Default = 0,
        Flags = 1,
        Nullable = 2
    }
}