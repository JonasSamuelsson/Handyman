using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.Validation
{
    internal class AnyValidator : ITypeInfoValidator
    {
        public bool TryValidate(TypeInfo actual, TypeInfo expected, ValidationContext context)
        {
            return expected is AnyTypeInfo;
        }
    }
}