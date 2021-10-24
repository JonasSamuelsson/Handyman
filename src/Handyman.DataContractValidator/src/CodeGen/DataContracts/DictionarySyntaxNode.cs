using Handyman.DataContractValidator.Model;
using System;

namespace Handyman.DataContractValidator.CodeGen.DataContracts
{
    internal class DictionarySyntaxNode : ISyntaxNode
    {
        public bool IsMultiLine => Value.IsMultiLine;

        public ISyntaxNode Key { get; set; }
        public ISyntaxNode Value { get; set; }

        public void GenerateCode(CodeBuilder builder)
        {
            if (IsMultiLine)
            {
                GenerateMultiLine(builder);
            }
            else
            {
                GenerateSingleLine(builder);
            }
        }

        private void GenerateMultiLine(CodeBuilder builder)
        {
            builder
                .Add("new Dictionary<")
                .Add(Key)
                .Add(">")
                .AddLineBreak()
                .Add("{")
                .AddLineBreak()
                .IncreaseIndentation()
                .Add(Value)
                .AddLineBreak()
                .DecreaseIndentation()
                .Add("}");
        }

        private void GenerateSingleLine(CodeBuilder builder)
        {
            builder
                .Add("new Dictionary<")
                .Add(Key)
                .Add("> { ")
                .Add(Value)
                .Add(" }");
        }

        public static ISyntaxNode Create(DictionaryTypeInfo typeInfo)
        {
            if (!typeInfo.Key.IsPrimitive)
            {
                throw new InvalidOperationException();
            }

            var value = typeInfo.Value.GetDataContractSyntaxNode();

            if (typeInfo.Value.IsPrimitive)
            {
                value = new TypeOfSyntaxNode
                {
                    Value = value
                };
            }

            return new DictionarySyntaxNode
            {
                Key = typeInfo.Key.GetDataContractSyntaxNode(),
                Value = value
            };
        }
    }
}