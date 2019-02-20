using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.Collections
{
    internal class CollectionHandler : IHandler
    {
        public bool TryGetTypeName(object o, ValidationContext context, out string name)
        {
            name = null;

            if (!TryGetItemTypeName(o, context, out var itemTypeName))
                return false;

            name = "enumerable";
            return true;
        }

        private bool TryGetItemTypeName(object o, ValidationContext context, out string name)
        {
            if (o is Type type && type.IsGenericType && type.TryGetInterfaceClosing(typeof(IEnumerable<>), out var @interface))
            {
                var itemType = @interface.GetGenericArguments().Single();
                name = context.GetTypeName(itemType);
                return true;
            }

            if (o.GetType().TryGetInterfaceClosing(typeof(IEnumerable<>), out _))
            {
                var item = ((IEnumerable)o).Cast<object>().Single();
                name = context.GetTypeName(item);
                return true;
            }

            name = null;
            return false;
        }

        public bool TryGetValidator(object dataContract, out IValidator validator)
        {
            if (dataContract is Type type && type.TryGetInterfaceClosing(typeof(IEnumerable<>), out var @interface))
            {
                var itemDataContract = @interface.GetGenericArguments().Single();
                validator = new CollectionValidator(itemDataContract);
                return true;
            }

            if (dataContract.GetType().TryGetInterfaceClosing(typeof(IEnumerable<>), out @interface))
            {
                var itemDataContract = ((IEnumerable)dataContract).Cast<object>().SingleOrDefault()
                    ?? @interface.GetGenericArguments().Single();
                validator = new CollectionValidator(itemDataContract);
                return true;
            }

            validator = null;
            return false;
        }
    }
}