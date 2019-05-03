using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.Dictionaries
{
    internal class DictionaryHandler : IHandler
    {
        public bool TryGetTypeName(object o, ValidationContext context, out string name)
        {
            name = null;
            return false;
        }

        public bool TryGetValidator(object dataContract, out IValidator validator)
        {
            if (dataContract is Type type && type.TryGetInterfaceClosing(typeof(IDictionary<,>), out var @interface))
            {
                var genericArguments = @interface.GetGenericArguments();
                var keyDataContract = genericArguments[0];
                var valueDataContract = genericArguments[1];
                validator = new DictionaryValidator(keyDataContract, valueDataContract);
                return true;
            }

            if (dataContract is Hashtable hashtable)
            {
                var entry = hashtable.Cast<DictionaryEntry>().Single();
                validator = new DictionaryValidator(entry.Key, entry.Value);
                return true;
            }

            validator = null;
            return false;
        }
    }
}