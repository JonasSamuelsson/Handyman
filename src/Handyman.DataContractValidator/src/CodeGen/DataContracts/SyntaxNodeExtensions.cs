namespace Handyman.DataContractValidator.CodeGen.DataContracts
{
    internal static class SyntaxNodeExtensions
    {
        public static ISyntaxNode WrapWithTypeOfIfValueSyntaxNode(this ISyntaxNode node)
        {
            return node is ValueSyntaxNode value
                ? new TypeOfSyntaxNode { Value = value }
                : node;
        }

        public static bool TryWrapWithTypeOfSyntaxNode(this ISyntaxNode node, out ISyntaxNode typeOfNode)
        {
            if (node is ValueSyntaxNode value)
            {
                typeOfNode = new TypeOfSyntaxNode
                {
                    Value = value
                };
                return true;
            }

            typeOfNode = null;
            return false;
        }
    }
}