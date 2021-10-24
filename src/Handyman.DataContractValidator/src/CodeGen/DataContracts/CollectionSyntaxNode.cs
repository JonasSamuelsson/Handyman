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
            var item = typeInfo.Item.GetDataContractSyntaxNode();

            if (typeInfo.Item.IsPrimitive)
            {
                item = new TypeOfSyntaxNode
                {
                    Value = item
                };
            }

            return new CollectionSyntaxNode
            {
                Item = item
            };
        }
    }
}