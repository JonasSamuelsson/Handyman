using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class NullableTypeInfoResolver : ITypeInfoResolver
    {
        public bool TryResolveTypeInfo(object o, TypeInfoResolverContext context, out TypeInfo typeInfo)
        {
            if (o is Nullable nullable)
            {
                typeInfo = context.GetTypeInfo(nullable.Item);
                typeInfo.IsNullable = true;
                return true;
            }

            typeInfo = null;
            return false;
        }
    }
}