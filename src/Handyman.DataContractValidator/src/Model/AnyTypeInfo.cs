using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.CodeGen.DataContracts;
using Handyman.DataContractValidator.Validation;

namespace Handyman.DataContractValidator.Model
{
    internal class AnyTypeInfo : TypeInfo
    {
        public override bool IsPrimitive => false;
        public override bool IsReference => true;
        public override string TypeName => "any";

        public override ITypeInfoValidator GetValidator()
        {
            return new AnyValidator();
        }

        public override ISyntaxNode GetDataContractSyntaxNode()
        {
            return new AnySyntaxNode();
        }
    }
}