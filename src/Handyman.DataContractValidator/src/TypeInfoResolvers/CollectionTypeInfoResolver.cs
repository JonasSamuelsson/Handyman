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
        public bool TryResolveTypeInfo(object o, TypeInfoResolverContext context, out TypeInfo typeInfo)
        {
            var isType = o is Type;
            var type = isType ? (Type)o : o.GetType();

            if (!type.TryGetInterfaceClosing(typeof(IEnumerable<>), out var @interface))
            {
                typeInfo = null;
                return false;
            }

            object item = @interface.GetGenericArguments().Single();

            if (!isType)
            {
                item = ((IEnumerable)o).OfType<object>().Single();
            }

            typeInfo = new CollectionTypeInfo
            {
                Item = context.GetTypeInfo(item)
            };
            return true;
        }
    }
}