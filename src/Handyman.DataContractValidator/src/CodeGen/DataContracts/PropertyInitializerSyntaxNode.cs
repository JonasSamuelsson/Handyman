namespace Handyman.DataContractValidator.CodeGen.DataContracts
{
    internal class PropertyInitializerSyntaxNode : ISyntaxNode
    {
        public bool IsMultiLine => PropertyValue.IsMultiLine;

        public string PropertyName { get; set; }
        public ISyntaxNode PropertyValue { get; set; }

        public void GenerateCode(CodeBuilder builder)
        {
            builder
                .Add($"{PropertyName} = ")
                .Add(PropertyValue);
        }
    }
}