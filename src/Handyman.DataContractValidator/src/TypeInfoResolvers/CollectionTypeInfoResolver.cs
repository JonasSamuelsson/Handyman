using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class CollectionTypeInfoResolver : ITypeInfoResolver
    {
        public ITypeInfo ResolveTypeInfo(object o, TypeInfoResolverContext context, Func<object, ITypeInfo> next)
        {
            var isType = o is Type;
            var type = isType ? (Type)o : o.GetType();

            if (!type.TryGetInterfaceClosing(typeof(IEnumerable<>), out var @interface))
            {
                return next(o);
            }

            object item = @interface.GetGenericArguments().Single();

            if (!isType)
            {
                item = ((IEnumerable)o).OfType<object>().Single();
            }

            return new CollectionTypeInfo
            {
                Item = context.GetTypeInfo(item)
            };
        }
    }
}