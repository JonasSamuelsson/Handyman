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

                var ids = System.Enum.GetValues(type)
                    .Cast<object>()
                    .Select(x => (long?)Convert.ChangeType(x, typeof(long)))
                    .OrderBy(x => x)
                    .ToList();

                var names = ids
                    .Select(x => System.Enum.GetName(type, x))
                    .ToList();

                typeInfo = CreateEnumTypeInfo(isFlags, isNullable, ids, names);
                return true;
            }

            if (o is Enum @enum)
            {
                var ids = @enum.HasIds ? @enum.Ids.Cast<long?>().ToList() : null;
                var names = @enum.HasNames ? @enum.Names.ToList() : null;
                typeInfo = CreateEnumTypeInfo(@enum.Flags, @enum.Nullable, ids, names);
                return true;
            }

            return false;
        }

        private static EnumTypeInfo CreateEnumTypeInfo(bool isFlags, bool isNullable, IReadOnlyList<long?> ids, IReadOnlyList<string> names)
        {
            if (ids?.Any() != true && names?.Any() != true)
            {
                throw new InvalidOperationException("An enum must have ids and/or names specified.");
            }

            List<EnumTypeInfo.Value> values;

            if (ids?.Any() != true)
            {
                values = names
                    .Select(x => new EnumTypeInfo.Value
                    {
                        Id = null,
                        Name = x
                    })
                    .ToList();
            }
            else if (names?.Any() != true)
            {
                values = ids
                    .Select(x => new EnumTypeInfo.Value
                    {
                        Id = x,
                        Name = null
                    })
                    .ToList();
            }
            else
            {
                if (ids.Count != names.Count)
                {
                    throw new InvalidOperationException();
                }

                values = ids
                    .Select((x, i) => new EnumTypeInfo.Value
                    {
                        Id = x,
                        Name = names[i]
                    })
                    .ToList();
            }

            return new EnumTypeInfo
            {
                IsFlags = isFlags,
                IsNullable = isNullable,
                Values = values
            };
        }
    }
}