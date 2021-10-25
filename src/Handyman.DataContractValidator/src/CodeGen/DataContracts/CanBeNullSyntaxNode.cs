namespace Handyman.DataContractValidator.CodeGen.DataContracts
{
    internal class CanBeNullSyntaxNode : ISyntaxNode
    {
        public bool IsMultiLine => Value.IsMultiLine;
        public ISyntaxNode Value { get; set; }

        public void GenerateCode(CodeBuilder builder)
        {
            builder
                .Add("new Nullable(")
                .Add(Value)
                .Add(")");
        }
    }
}