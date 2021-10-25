using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class CanBeNullTypeInfoResolver : ITypeInfoResolver
    {
        public bool TryResolveTypeInfo(object o, TypeInfoResolverContext context, out TypeInfo typeInfo)
        {
            if (o is CanBeNull canBeNull)
            {
                typeInfo = context.GetTypeInfo(canBeNull.Item);
                typeInfo.IsNullable = true;
                return true;
            }

            typeInfo = null;
            return false;
        }
    }
}