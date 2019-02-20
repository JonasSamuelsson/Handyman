using System;
using System.Collections.Generic;
using System.Linq;

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
            var context = new ValidationContext();

            errors = context.Validate(type, dataContract);

            return !errors.Any();
        }
    }
}
