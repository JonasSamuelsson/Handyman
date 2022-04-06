using Handyman.DataContractValidator.TypeInfoResolvers;
using Handyman.DataContractValidator.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator
{
    public class DataContractValidator
    {
        public void Validate(Type type, object dataContract)
        {
            Validate(type, dataContract, new ValidationOptions());
        }

        public void Validate(Type type, object dataContract, ValidationOptions options)
        {
            if (Validate(type, dataContract, out var errors))
                return;

            throw new ValidationException(string.Join(Environment.NewLine, errors));
        }

        public bool Validate(Type type, object dataContract, out IEnumerable<string> errors)
        {
            return Validate(type, dataContract, new ValidationOptions(), out errors);
        }

        public bool Validate(Type type, object dataContract, ValidationOptions options, out IEnumerable<string> errors)
        {
            var resolverContext = new TypeInfoResolverContext();

            var actual = resolverContext.GetTypeInfo(type);
            var expected = resolverContext.GetTypeInfo(dataContract);

            var validationContext = new ValidationContext
            {
                Options = options
            };

            validationContext.Validate(actual, expected);

            errors = validationContext.Errors;

            return !validationContext.Errors.Any();
        }
    }
}