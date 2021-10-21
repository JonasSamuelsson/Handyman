using Handyman.DataContractValidator.CodeGen;

namespace Handyman.DataContractValidator.Tests.CodeGen
{
    internal static class SyntaxNodeExtensions
    {
        public static string GenerateCode(this ISyntaxNode node)
        {
            var builder = new CodeBuilder();
            node.GenerateCode(builder);
            return builder.Build();
        }
    }
}