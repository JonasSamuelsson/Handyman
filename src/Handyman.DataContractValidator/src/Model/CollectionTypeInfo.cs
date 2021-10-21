using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.CodeGen.DataContracts;
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

        public override ISyntaxNode GetDataContractSyntaxNode()
        {
            return CollectionSyntaxNode.Create(this);
        }
    }
}