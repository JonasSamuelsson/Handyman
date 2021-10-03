using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal interface ITypeInfoResolver
    {
        bool TryResolveTypeInfo(object o, TypeInfoResolverContext context, out TypeInfo typeInfo);
    }
}