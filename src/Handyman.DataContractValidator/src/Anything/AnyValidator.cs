using System;

namespace Handyman.DataContractValidator.Anything
{
    internal class AnyValidator : IValidator
    {
        public string GetTypeName(ValidationContext context)
        {
            throw new NotImplementedException();
        }

        public void Validate(Type type, ValidationContext context)
        {
            // do nothing
        }
    }
}