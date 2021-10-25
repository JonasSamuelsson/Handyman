using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.CodeGen.DataContracts;
using Handyman.DataContractValidator.Validation;

namespace Handyman.DataContractValidator.Model
{
    internal class CollectionTypeInfo : TypeInfo
    {
        public ITypeInfo Item { get; set; }

        public override bool IsPrimitive => false;
        public override bool IsReference => true;

        protected override string GetName()
        {
            return "enumerable";
        }

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