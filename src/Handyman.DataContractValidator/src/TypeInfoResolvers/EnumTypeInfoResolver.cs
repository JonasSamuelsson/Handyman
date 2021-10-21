using Handyman.DataContractValidator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeInfo = Handyman.DataContractValidator.Model.TypeInfo;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class EnumTypeInfoResolver : ITypeInfoResolver
    {
        public bool TryResolveTypeInfo(object o, TypeInfoResolverContext context, out TypeInfo typeInfo)
        {
            typeInfo = null;

            if (o is Type type)
            {
                if (!type.IsValueType)
                {
                    return false;
                }

                var isNullable = false;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    isNullable = true;
                    type = type.GetGenericArguments().Single();
                }

                if (!type.IsEnum)
                {
                    return false;
                }

                var isFlags = type.GetCustomAttributes<FlagsAttribute>().Any();

                var values = System.Enum.GetValues(type)
                    .Cast<object>()
                    .Select(x => (long)Convert.ChangeType(x, typeof(long)))
                    .OrderBy(x => x)
                    .ToDictionary(x => x, x => System.Enum.GetName(type, x));

                typeInfo = CreateEnumTypeInfo(isFlags, isNullable, values);
                return true;
            }

            if (o is Enum @enum)
            {
                typeInfo = CreateEnumTypeInfo(@enum.Flags, @enum.Nullable, @enum.Values);
                return true;
            }

            return false;
        }

        private static EnumTypeInfo CreateEnumTypeInfo(bool isFlags, bool isNullable, Dictionary<long, string> values)
        {
            return new EnumTypeInfo
            {
                IsFlags = isFlags,
                IsNullable = isNullable,
                Values = values
            };
        }
    }
}