using Handyman.DataContractValidator.Model;
using System;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class TypeInfoResolverContext
    {
        private readonly IEnumerable<ITypeInfoResolver> _resolvers = new ITypeInfoResolver[]
        {
            new AnyTypeInfoResolver(),
            new EnumTypeInfoResolver(),
            new ValueTypeInfoResolver(),
            new DictionaryTypeInfoResolver(),
            new CollectionTypeInfoResolver(),
            new ObjectTypeInfoResolver()
        };

        public TypeInfo GetTypeInfo(object o)
        {
            if (o is DataContractReference dcr)
            {
                o = dcr.Resolve();
            }

            foreach (var resolver in _resolvers)
            {
                if (!resolver.TryResolveTypeInfo(o, this, out var typeInfo))
                    continue;

                return typeInfo;
            }

            throw new InvalidOperationException("Unable to resolve type info.");
        }
    }
}