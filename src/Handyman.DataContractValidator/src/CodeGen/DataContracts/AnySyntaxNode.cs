namespace Handyman.DataContractValidator.CodeGen.DataContracts
{
    internal class AnySyntaxNode : ISyntaxNode
    {
        public bool IsMultiLine => false;

        public void GenerateCode(CodeBuilder builder)
        {
            builder.Add("new Any()");
        }
    }
}