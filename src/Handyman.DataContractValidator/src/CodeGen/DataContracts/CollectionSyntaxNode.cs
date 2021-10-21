using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.CodeGen.DataContracts
{
    internal class CollectionSyntaxNode : ISyntaxNode
    {
        public bool IsMultiLine => Item.IsMultiLine;

        public ISyntaxNode Item { get; set; }

        public void GenerateCode(CodeBuilder builder)
        {
            if (IsMultiLine)
            {
                builder
                    .Add("new []")
                    .AddLineBreak()
                    .Add("{")
                    .AddLineBreak()
                    .IncreaseIndentation()
                    .Add(Item)
                    .AddLineBreak()
                    .DecreaseIndentation()
                    .Add("}");
            }
            else
            {
                builder
                    .Add("new [] { ")
                    .Add(Item)
                    .Add(" }");
            }
        }

        public static ISyntaxNode Create(CollectionTypeInfo typeInfo)
        {
            return new CollectionSyntaxNode
            {
                Item = typeInfo.Item.GetDataContractSyntaxNode().WrapWithTypeOfIfValueSyntaxNode()
            };
        }
    }
}