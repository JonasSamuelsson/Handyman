using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator
{
    public class FlagsEnum : Enum
    {
        public FlagsEnum(params long[] values)
            : base(true, values)
        { }
    }

    public class Enum
    {
        public Enum(params long[] values)
            : this(false, values)
        { }

        protected Enum(bool isFlagsEnum, IEnumerable<long> values)
        {
            IsFlagsEnum = isFlagsEnum;
            EnumValues = values.ToList();
        }

        internal IEnumerable<long> EnumValues { get; }
        internal bool IsFlagsEnum { get; set; }
    }

    public class Enum<T> : Enum where T : struct
    {
        internal Enum() : base(GetIsFlagsEnum(), GetValues())
        {
        }

        private static bool GetIsFlagsEnum()
        {
            return typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).Any();
        }

        private static IEnumerable<long> GetValues()
        {
            return System.Enum.GetValues(typeof(T)).Cast<long>();
        }
    }
}