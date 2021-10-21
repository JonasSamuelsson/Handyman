using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.CodeGen.DataContracts;
using Handyman.DataContractValidator.Validation;
using System;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.Model
{
    internal class ValueTypeInfo : TypeInfo
    {
        public Type Value { get; set; }

        public override string TypeName
        {
            get
            {
                var name = TypeNames.TryGetValue(Value, out var s) ? s : Value.Name;
                return $"{name}{(IsNullable == true ? "?" : "")}";
            }
        }

        public override ITypeInfoValidator GetValidator()
        {
            return new ValueValidator();
        }

        public override ISyntaxNode GetDataContractSyntaxNode()
        {
            return new ValueSyntaxNode
            {
                TypeName = TypeName
            };
        }

        private static readonly Dictionary<Type, string> TypeNames = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(string), "string" }
        };
    }
}