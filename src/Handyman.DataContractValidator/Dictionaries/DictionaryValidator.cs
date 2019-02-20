using System;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.Dictionaries
{
    internal class DictionaryValidator : IValidator
    {
        private readonly object _keyDataContract;
        private readonly object _valueDataContract;

        public DictionaryValidator(object keyDataContract, object valueDataContract)
        {
            _keyDataContract = keyDataContract;
            _valueDataContract = valueDataContract;
        }

        public string GetTypeName(ValidationContext context)
        {
            throw new NotImplementedException();
        }

        public void Validate(Type type, ValidationContext context)
        {
            if (!type.TryGetInterfaceClosing(typeof(IDictionary<,>), out var @interface))
            {
                context.AddError($"type mismatch, expected dictionary but found {context.GetTypeName(type)}");
                return;
            }

            using (context.CreateScope("Key"))
                context.Validate(@interface.GetGenericArguments()[0], _keyDataContract);

            using (context.CreateScope("Value"))
                context.Validate(@interface.GetGenericArguments()[1], _valueDataContract);
        }
    }
}