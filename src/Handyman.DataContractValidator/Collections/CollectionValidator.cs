using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.Collections
{
    internal class CollectionValidator : IValidator
    {
        private readonly object _itemDataContract;

        public CollectionValidator(object itemDataContract)
        {
            _itemDataContract = itemDataContract;
        }

        public string GetTypeName(ValidationContext context)
        {
            return "enumerable";
        }

        public void Validate(Type type, ValidationContext context)
        {
            if (!type.TryGetInterfaceClosing(typeof(IEnumerable<>), out var @interface))
            {
                context.AddError($"type mismatch, expected enumerable but found {context.GetTypeName(type)}");
                return;
            }

            using (context.CreateScope("Item"))
                context.Validate(@interface.GetGenericArguments().Single(), _itemDataContract);
        }
    }
}