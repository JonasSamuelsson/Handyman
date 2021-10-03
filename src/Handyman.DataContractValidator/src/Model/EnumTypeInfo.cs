using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.Model
{
    internal class EnumTypeInfo : TypeInfo
    {
        public bool IsFlags { get; set; }
        public IEnumerable<Value> Values { get; set; }

        public bool HasIds => Values.Any(x => x.Id != null);
        public bool HasNames => Values.Any(x => x.Name != null);

        public IEnumerable<long> Ids => Values.Select(x => (long)x.Id);
        public IEnumerable<string> Names => Values.Select(x => x.Name);

        public override string TypeName => "enum";

        public struct Value
        {
            public long? Id { get; set; }
            public string Name { get; set; }
        }

        //internal static EnumTypeInfo EnumToTypeInfo(Type type)
        //{
        //    if (!type.IsValueType)
        //    {
        //        throw new ArgumentException();
        //    }

        //    var isNullable = !type.IsEnum;
        //    type = type.IsGenericType ? type.GetGenericArguments().Single() : type;

        //    if (!type.IsEnum)
        //    {
        //        throw new ArgumentException();
        //    }

        //    var isFlags = type.GetCustomAttributes<FlagsAttribute>().Any();

        //    var values = System.Enum.GetValues(type)
        //        .Cast<object>()
        //        .Select(x => new Value
        //        {
        //            Id = (long)Convert.ChangeType(x, typeof(long)),
        //            Name = System.Enum.GetName(type, x)
        //        })
        //        .ToList();

        //    return new EnumTypeInfo
        //    {
        //        IsFlags = isFlags,
        //        IsNullable = isNullable,
        //        Values = values
        //    };
        //}
    }
}