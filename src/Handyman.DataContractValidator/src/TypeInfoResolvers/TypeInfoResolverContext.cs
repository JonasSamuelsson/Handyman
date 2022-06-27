using Handyman.DataContractValidator.Model;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class TypeInfoResolverContext
    {
        private readonly IReadOnlyList<ITypeInfoResolver> _typeInfoResolvers = CreateTypeInfoResolvers();

        public ITypeInfo GetTypeInfo(object o)
        {
            if (o is DataContractReference dataContractReference)
            {
                o = dataContractReference.Resolve();
            }

            return new TypeInfoResolverPipeline(_typeInfoResolvers, this).Next(o);
        }

        private static IReadOnlyList<ITypeInfoResolver> CreateTypeInfoResolvers()
        {
            return new ITypeInfoResolver[]
            {
                new RecursiveTypeInfoResolver(),
                new CanBeNullTypeInfoResolver(),
                new AnyTypeInfoResolver(),
                new EnumTypeInfoResolver(),
                new ValueTypeInfoResolver(),
                new DictionaryTypeInfoResolver(),
                new CollectionTypeInfoResolver(),
                new ObjectTypeInfoResolver(),
                new TypeInfoResolutionFailedResolver()
            };
        }
    }
}