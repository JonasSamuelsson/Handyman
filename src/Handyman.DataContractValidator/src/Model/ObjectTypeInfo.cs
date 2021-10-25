using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.CodeGen.DataContracts;
using Handyman.DataContractValidator.Validation;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.Model
{
    internal class ObjectTypeInfo : TypeInfo
    {
        public IEnumerable<PropertyInfo> Properties { get; set; }

        public override bool IsPrimitive => false;
        public override bool IsReference => true;

        protected override string GetName()
        {
            return "object";
        }

        public override ITypeInfoValidator GetValidator()
        {
            return new ObjectValidator();
        }

        public override ISyntaxNode GetDataContractSyntaxNode()
        {
            return ObjectSyntaxNode.Create(this);
        }
    }
}