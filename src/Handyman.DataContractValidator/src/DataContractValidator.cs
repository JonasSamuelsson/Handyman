using Handyman.DataContractValidator.TypeInfoResolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using ValidationContext = Handyman.DataContractValidator.Validation.ValidationContext;

namespace Handyman.DataContractValidator
{
    public class DataContractValidator
    {
        public void Validate(Type type, object dataContract)
        {
            if (Validate(type, dataContract, out var errors))
                return;

            throw new ValidationException(string.Join(Environment.NewLine, errors));
        }

        public bool Validate(Type type, object dataContract, out IEnumerable<string> errors)
        {
            var resolverContext = new TypeInfoResolverContext();

            var actual = resolverContext.GetTypeInfo(type);
            var expected = resolverContext.GetTypeInfo(dataContract);

            var validationContext = new ValidationContext();

            validationContext.Validate(actual, expected);

            errors = validationContext.Errors;

            return !validationContext.Errors.Any();
        }
    }
}
