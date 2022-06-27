using Handyman.DataContractValidator.Model;
using System;
using System.Linq;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class ValueTypeInfoResolver : ITypeInfoResolver
    {
        public ITypeInfo ResolveTypeInfo(object o, TypeInfoResolverContext context, Func<object, ITypeInfo> next)
        {
            if (o is Type type)
            {
                if (type == typeof(string))
                {
                    return new ValueTypeInfo { Type = typeof(string) };
                }

                if (type.IsValueType)
                {
                    var isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

                    return new ValueTypeInfo
                    {
                        IsNullable = isNullable,
                        Type = isNullable ? type.GetGenericArguments().Single() : type
                    };
                }
            }

            return next(o);
        }
    }
}