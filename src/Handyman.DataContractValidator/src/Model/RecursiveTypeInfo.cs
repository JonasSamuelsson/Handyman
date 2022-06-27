using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.Validation;
using System;

namespace Handyman.DataContractValidator.Model
{
    internal class RecursiveTypeInfo : TypeInfo
    {
        public ITypeInfo InnerTypeInfo { get; set; }

        public override bool IsPrimitive { get; }
        public override bool IsReference { get; }

        protected override string GetName()
        {
            throw new NotImplementedException();
        }

        public override ITypeInfo GetValidatableTypeInfo()
        {
            return InnerTypeInfo;
        }

        public override ITypeInfoValidator GetValidator()
        {
            throw new NotImplementedException();
        }

        public override ISyntaxNode GetDataContractSyntaxNode()
        {
            throw new InvalidOperationException("DataContractGenerator does not support recursive types.");
        }
    }
}