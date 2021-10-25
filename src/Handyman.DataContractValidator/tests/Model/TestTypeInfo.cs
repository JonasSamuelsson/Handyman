using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.Validation;
using System;

namespace Handyman.DataContractValidator.Tests.Model
{
    internal class TestTypeInfo : ITypeInfo
    {
        public bool? IsNullable { get; set; }
        public bool IsPrimitive { get; set; }
        public bool IsReference { get; set; }
        public string? Name { get; set; }
        public ISyntaxNode? DataContractSyntaxNode { get; set; }
        public ITypeInfoValidator? Validator { get; set; }

        public ITypeInfoValidator GetValidator() => Validator ?? throw new InvalidOperationException();
        public ISyntaxNode GetDataContractSyntaxNode() => DataContractSyntaxNode ?? throw new InvalidOperationException();
    }
}