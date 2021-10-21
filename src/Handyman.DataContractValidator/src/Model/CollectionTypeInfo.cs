using Handyman.DataContractValidator.Validation;

namespace Handyman.DataContractValidator.Model
{
    internal class CollectionTypeInfo : TypeInfo
    {
        public TypeInfo Item { get; set; }
        public override string TypeName => "enumerable";

        public override ITypeInfoValidator GetValidator()
        {
            return new CollectionValidator();
        }
    }
}