using Handyman.DataContractValidator.Model;
using System;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class AnyTypeInfoResolver : ITypeInfoResolver
    {
        public bool TryResolveTypeInfo(object o, TypeInfoResolverContext context, out TypeInfo typeInfo)
        {
            if ((o as Type ?? o.GetType()) == typeof(Any))
            {
                typeInfo = new AnyTypeInfo();
                return true;
            }

            typeInfo = null;
            return false;
        }
    }
}