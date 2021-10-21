using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.Validation;
using System;

namespace Handyman.DataContractValidator.Model
{
    internal abstract class TypeInfo
    {
        public bool? IsNullable { get; set; }
        public virtual string TypeName => throw new NotImplementedException();

        public abstract ITypeInfoValidator GetValidator();
        public abstract ISyntaxNode GetDataContractSyntaxNode();
    }
}