using System;

namespace Handyman.DataContractValidator.Values
{
    internal class ValueValidator : IValidator
    {
        private readonly Type _type;

        public ValueValidator(Type type)
        {
            _type = type;
        }

        public string GetTypeName(ValidationContext context)
        {
            return ValueHandler.GetName(_type);
        }

        public void Validate(Type type, ValidationContext context)
        {
            if (type == _type)
                return;

            var expected = context.GetTypeName(_type);
            var actual = context.GetTypeName(type);
            context.AddError($"type mismatch, expected {expected} but found {actual}");
        }
    }
}