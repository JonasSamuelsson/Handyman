using Handyman.DataContractValidator.Model;
using System;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class TypeInfoResolutionFailedResolver : ITypeInfoResolver
    {
        public ITypeInfo ResolveTypeInfo(object o, TypeInfoResolverContext context, Func<object, ITypeInfo> next)
        {
            throw new InvalidOperationException("Could not resolve type info.");
        }
    }
}