namespace Handyman.DataContractValidator.CodeGen.DataContracts
{
    internal class TypeOfSyntaxNode : ISyntaxNode
    {
        public bool IsMultiLine => false;

        public ISyntaxNode Value { get; set; }

        public void GenerateCode(CodeBuilder builder)
        {
            builder
                .Add("typeof(")
                .Add(Value)
                .Add(")");
        }
    }
}