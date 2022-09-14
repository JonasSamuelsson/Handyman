using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.Validation
{
    internal interface ITypeInfoValidator
    {
        void Validate(ITypeInfo actual, ITypeInfo expected, DataContractValidatorContext context);
    }
}