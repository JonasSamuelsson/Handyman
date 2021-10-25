using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.Validation;

namespace Handyman.DataContractValidator.Model
{
    internal abstract class TypeInfo : ITypeInfo
    {
        public bool? IsNullable { get; set; }
        public abstract bool IsPrimitive { get; }
        public abstract bool IsReference { get; }
        public virtual string Name => $"{GetName()}{(IsNullable == true ? "?" : "")}";

        protected abstract string GetName();
        public abstract ITypeInfoValidator GetValidator();
        public abstract ISyntaxNode GetDataContractSyntaxNode();
    }
}