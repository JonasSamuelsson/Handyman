using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.CodeGen.DataContracts;
using Handyman.DataContractValidator.Validation;
using System;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.Model
{
    internal class ValueTypeInfo : TypeInfo
    {
        public Type Type { get; set; }

        public override bool IsPrimitive => true;
        public override bool IsReference => !Type.IsValueType;
        public override string TypeName => GetTypeNameWithNullableIndicator();

        private string GetTypeNameWithNullableIndicator()
        {
            return $"{GetTypeNameBase()}{(IsNullable == true ? "?" : "")}";
        }

        private string GetTypeNameBase()
        {
            return TypeNames.TryGetValue(Type, out var s) ? s : Type.Name;
        }

        public override ITypeInfoValidator GetValidator()
        {
            return new ValueValidator();
        }

        public override ISyntaxNode GetDataContractSyntaxNode()
        {
            return new ValueSyntaxNode
            {
                TypeName = GetSyntaxNodeTypeName()
            };
        }

        private string GetSyntaxNodeTypeName()
        {
            return IsReference
                ? GetTypeNameBase()
                : GetTypeNameWithNullableIndicator();
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