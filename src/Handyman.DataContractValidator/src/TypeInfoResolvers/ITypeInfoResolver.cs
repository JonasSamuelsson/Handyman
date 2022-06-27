using Handyman.DataContractValidator.Model;
using System;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal interface ITypeInfoResolver
    {
        ITypeInfo ResolveTypeInfo(object o, TypeInfoResolverContext context, Func<object, ITypeInfo> next);
    }
}