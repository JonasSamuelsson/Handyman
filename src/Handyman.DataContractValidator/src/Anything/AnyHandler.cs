using System;

namespace Handyman.DataContractValidator.Anything
{
    internal class AnyHandler : IHandler
    {
        private static readonly AnyValidator Validator = new AnyValidator();

        public bool TryGetTypeName(object o, ValidationContext context, out string name)
        {
            name = null;
            return false;
        }

        public bool TryGetValidator(object dataContract, out IValidator validator)
        {
            if (dataContract is Any || (dataContract is Type type && type == typeof(Any)))
            {
                validator = Validator;
                return true;
            }

            validator = null;
            return false;
        }
    }
}