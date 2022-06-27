using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.Validation;

namespace Handyman.DataContractValidator.Model
{
    internal interface ITypeInfo
    {
        bool? IsNullable { get; set; }
        bool IsPrimitive { get; }
        bool IsReference { get; }
        string Name { get; }

        ITypeInfo GetValidatableTypeInfo();
        ITypeInfoValidator GetValidator();
        ISyntaxNode GetDataContractSyntaxNode();
    }
}