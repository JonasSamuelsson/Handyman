using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.Values
{
    internal class ValueHandler : IHandler
    {
        private static readonly Dictionary<Type, string> TypeNames = new Dictionary<Type, string>
        {
            {typeof(bool), "bool"},
            {typeof(byte), "byte"},
            {typeof(char), "char"},
            {typeof(DateTime), "datetime"},
            {typeof(DateTimeOffset), "datetimeoffset"},
            {typeof(decimal), "decimal"},
            {typeof(double), "double"},
            {typeof(float), "float"},
            {typeof(Guid), "guid"},
            {typeof(int), "int"},
            {typeof(long), "long"},
            {typeof(short), "short"},
            {typeof(string), "string"},
            {typeof(TimeSpan), "timespan"},
            {typeof(ushort), "ushort"},
            {typeof(uint), "uint"},
            {typeof(ulong), "ulong"}
        };

        public static string GetName(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? $"{GetName(type.GetGenericArguments().Single())}?"
                : TypeNames.TryGetValue(type, out var name)
                    ? name
                    : type.Name;
        }

        public bool TryGetTypeName(object o, ValidationContext context, out string name)
        {
            name = null;

            if (o is Type type && (type.IsValueType || type == typeof(string)))
            {
                name = GetName(type);
                return true;
            }

            return false;
        }

        public bool TryGetValidator(object dataContract, out IValidator validator)
        {
            if (dataContract is Type type && TypeNames.ContainsKey(type))
            {
                validator = new ValueValidator(type);
                return true;
            }

            if (dataContract is Value value)
            {
                validator = new ValueValidator(value.Type);
                return true;
            }

            validator = null;
            return false;
        }
    }
}