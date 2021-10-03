using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class DictionaryTypeInfoResolver : ITypeInfoResolver
    {
        public bool TryResolveTypeInfo(object o, TypeInfoResolverContext context, out TypeInfo typeInfo)
        {
            var isType = o is Type;
            var type = o as Type ?? o.GetType();

            if (type.TryGetInterfaceClosing(typeof(IDictionary<,>), out var @interface))
            {
                var genericArguments = @interface.GetGenericArguments();

                typeInfo = new DictionaryTypeInfo
                {
                    Key = context.GetTypeInfo(genericArguments[0]),
                    Value = context.GetTypeInfo(genericArguments[1])
                };

                return true;
            }

            if (!isType && o is Hashtable hashtable)
            {
                var entry = hashtable.OfType<DictionaryEntry>().Single();

                typeInfo = new DictionaryTypeInfo
                {
                    Key = context.GetTypeInfo(entry.Key),
                    Value = context.GetTypeInfo(entry.Value)
                };

                return true;
            }

            typeInfo = null;
            return false;
        }
    }
}