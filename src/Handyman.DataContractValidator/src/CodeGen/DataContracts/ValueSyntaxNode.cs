namespace Handyman.DataContractValidator.CodeGen.DataContracts
{
    internal class ValueSyntaxNode : ISyntaxNode
    {
        public bool IsMultiLine => false;
        public string TypeName { get; set; }

        public void GenerateCode(CodeBuilder builder)
        {
            builder.Add(TypeName);
        }
    }
}