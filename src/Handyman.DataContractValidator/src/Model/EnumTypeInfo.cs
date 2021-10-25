using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.CodeGen.DataContracts;
using Handyman.DataContractValidator.Validation;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.Model
{
    internal class EnumTypeInfo : TypeInfo
    {
        public bool IsFlags { get; set; }
        public override bool IsPrimitive => false;
        public override bool IsReference => false;
        public Dictionary<long, string> Values { get; set; }

        protected override string GetName()
        {
            return "enum";
        }

        public override ITypeInfoValidator GetValidator()
        {
            return new EnumValidator();
        }

        public override ISyntaxNode GetDataContractSyntaxNode()
        {
            return EnumSyntaxNode.Create(this);
        }
    }
}