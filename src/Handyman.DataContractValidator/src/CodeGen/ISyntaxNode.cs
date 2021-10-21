namespace Handyman.DataContractValidator.CodeGen
{
    internal interface ISyntaxNode
    {
        bool IsMultiLine { get; }

        void GenerateCode(CodeBuilder builder);
    }
}