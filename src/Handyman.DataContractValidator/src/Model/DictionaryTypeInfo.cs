using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.CodeGen.DataContracts;
using Handyman.DataContractValidator.Validation;

namespace Handyman.DataContractValidator.Model
{
    internal class DictionaryTypeInfo : TypeInfo
    {
        public ITypeInfo Key { get; set; }
        public ITypeInfo Value { get; set; }

        public override bool IsPrimitive => false;
        public override bool IsReference => true;

        protected override string GetName()
        {
            return "dictionary";
        }

        public override ITypeInfoValidator GetValidator()
        {
            return new DictionaryValidator();
        }

        public override ISyntaxNode GetDataContractSyntaxNode()
        {
            return DictionarySyntaxNode.Create(this);
        }
    }
}