using System;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.Model
{
    internal class ValueTypeInfo : TypeInfo
    {
        public Type Value { get; set; }

        public override string TypeName
        {
            get
            {
                var name = TypeNames.TryGetValue(Value, out var s) ? s : Value.Name;
                return $"{name}{(IsNullable == true ? "?" : "")}";
            }
        }

        private static readonly Dictionary<Type, string> TypeNames = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(string), "string" }
        };
    }
}