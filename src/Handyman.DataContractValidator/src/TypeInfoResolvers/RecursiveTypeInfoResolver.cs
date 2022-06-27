using Handyman.DataContractValidator.Model;
using System;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class RecursiveTypeInfoResolver : ITypeInfoResolver
    {
        private readonly Dictionary<object, RecursiveTypeInfo> _recursiveTypeInfos = new Dictionary<object, RecursiveTypeInfo>();
        private readonly Stack<object> _trace = new Stack<object>();

        public ITypeInfo ResolveTypeInfo(object o, TypeInfoResolverContext context, Func<object, ITypeInfo> next)
        {
            if (_recursiveTypeInfos.TryGetValue(o, out var recursiveTypeInfo))
            {
                return recursiveTypeInfo;
            }

            if (_trace.Contains(o))
            {
                recursiveTypeInfo = new RecursiveTypeInfo();

                _recursiveTypeInfos.Add(o, recursiveTypeInfo);

                return recursiveTypeInfo;
            }

            _trace.Push(o);

            var typeInfo = next(o);

            _trace.Pop();

            if (_recursiveTypeInfos.TryGetValue(o, out recursiveTypeInfo) && recursiveTypeInfo.InnerTypeInfo == null)
            {
                recursiveTypeInfo.InnerTypeInfo = typeInfo;

                return recursiveTypeInfo;
            }

            return typeInfo;
        }
    }
}