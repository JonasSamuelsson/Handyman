using Handyman.DataContractValidator.Model;

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
            return new DictionarySyntaxNode
            {
                Key = typeInfo.Key.GetDataContractSyntaxNode(),
                Value = typeInfo.Value.GetDataContractSyntaxNode().WrapWithTypeOfIfValueSyntaxNode()
            };
        }
    }
}