using Handyman.DataContractValidator.Model;
using System;
using System.Linq;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class ValueTypeInfoResolver : ITypeInfoResolver
    {
        public bool TryResolveTypeInfo(object o, TypeInfoResolverContext context, out TypeInfo typeInfo)
        {
            if (o is Type type)
            {
                if (type == typeof(string))
                {
                    typeInfo = new ValueTypeInfo { Type = typeof(string) };
                    return true;
                }

                if (type.IsValueType)
                {
                    var isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

                    typeInfo = new ValueTypeInfo
                    {
                        IsNullable = isNullable,
                        Type = isNullable ? type.GetGenericArguments().Single() : type
                    };
                    return true;
                }
            }

            typeInfo = null;
            return false;
        }
    }
}