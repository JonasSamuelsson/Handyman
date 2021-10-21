using Handyman.DataContractValidator.CodeGen;
using System;

namespace Handyman.DataContractValidator.Tests.CodeGen
{
    internal class TestSyntaxNode : ISyntaxNode
    {
        public string? Code { get; set; }
        public Action<CodeBuilder>? GenerateCodeAction { get; set; }
        public bool IsMultiLine { get; set; }

        public void GenerateCode(CodeBuilder builder)
        {
            if (Code != null)
            {
                builder.Add(Code);
            }
            else if (GenerateCodeAction != null)
            {
                GenerateCodeAction(builder);
            }
            else
            {
                var message = $"Both {nameof(Code)} nor {nameof(GenerateCodeAction)} has null values.";
                throw new InvalidOperationException(message);
            }
        }
    }
}