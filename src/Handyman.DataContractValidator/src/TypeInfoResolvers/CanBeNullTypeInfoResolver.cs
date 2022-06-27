using Handyman.DataContractValidator.Model;
using System;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class CanBeNullTypeInfoResolver : ITypeInfoResolver
    {
        public ITypeInfo ResolveTypeInfo(object o, TypeInfoResolverContext context, Func<object, ITypeInfo> next)
        {
            if (o is CanBeNull canBeNull)
            {
                var typeInfo = next(canBeNull.Item);
                typeInfo.IsNullable = true;
                return typeInfo;
            }

            return next(o);
        }
    }
}