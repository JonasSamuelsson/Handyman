using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.Validation
{
    class TypeInfosMatchValidator : ITypeInfoValidator
    {
        public bool TryValidate(TypeInfo actual, TypeInfo expected, ValidationContext context)
        {
            if (actual.GetType() != expected.GetType())
            {
                context.AddError($"type mismatch, expected '{expected.TypeName}' but found '{actual.TypeName}'.");
                return true;
            }

            return false;
        }
    }
}