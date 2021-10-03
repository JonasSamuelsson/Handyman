using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.Validation
{
    internal class DictionaryValidator : TypeInfoValidator<DictionaryTypeInfo>
    {
        internal override void Validate(DictionaryTypeInfo actual, DictionaryTypeInfo expected, ValidationContext context)
        {
            context.Validate("Key", actual.Key, expected.Key);
            context.Validate("Value", actual.Value, expected.Value);
        }
    }
}