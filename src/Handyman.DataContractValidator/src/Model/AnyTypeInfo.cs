using Handyman.DataContractValidator.Validation;

namespace Handyman.DataContractValidator.Model
{
    internal class AnyTypeInfo : TypeInfo
    {
        public override string TypeName => "any";

        public override ITypeInfoValidator GetValidator()
        {
            return new AnyValidator();
        }
    }
}