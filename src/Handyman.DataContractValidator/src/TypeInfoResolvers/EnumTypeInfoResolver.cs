using Handyman.DataContractValidator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class EnumTypeInfoResolver : ITypeInfoResolver
    {
        public ITypeInfo ResolveTypeInfo(object o, TypeInfoResolverContext context, Func<object, ITypeInfo> next)
        {
            if (o is Type type)
            {
                if (!type.IsValueType)
                {
                    return next(o);
                }

                var isNullable = false;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    isNullable = true;
                    type = type.GetGenericArguments().Single();
                }

                if (!type.IsEnum)
                {
                    return next(o);
                }

                var isFlags = type.GetCustomAttributes<FlagsAttribute>().Any();

                var values = System.Enum.GetValues(type)
                    .Cast<object>()
                    .Select(x => (long)Convert.ChangeType(x, typeof(long)))
                    .OrderBy(x => x)
                    .GroupBy(x => x, x => System.Enum.GetName(type, x))
                    .ToDictionary(x => x.Key, x => x.OrderBy(s => s).First());

                return CreateEnumTypeInfo(isFlags, isNullable, values);
            }

            if (o is Enum @enum)
            {
                return CreateEnumTypeInfo(@enum.Flags, @enum.Nullable, @enum.Values);
            }

            return next(o);
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