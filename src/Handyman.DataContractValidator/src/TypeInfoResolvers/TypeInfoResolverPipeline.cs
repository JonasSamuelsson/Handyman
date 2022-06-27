using Handyman.DataContractValidator.Model;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class TypeInfoResolverPipeline
    {
        private int _index;
        private readonly IReadOnlyList<ITypeInfoResolver> _typeInfoResolvers;
        private readonly TypeInfoResolverContext _context;

        public TypeInfoResolverPipeline(IReadOnlyList<ITypeInfoResolver> typeInfoResolvers, TypeInfoResolverContext context)
        {
            _index = -1;
            _typeInfoResolvers = typeInfoResolvers;
            _context = context;
        }

        public ITypeInfo Next(object o)
        {
            _index++;
            return _typeInfoResolvers[_index].ResolveTypeInfo(o, _context, Next);
        }
    }
}