using Handyman.DataContractValidator.CodeGen.DataContracts;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen.DataContracts
{
    public class TypeOfSyntaxNodeTests
    {
        [Fact]
        public void ShouldBeSingleLine()
        {
            new TypeOfSyntaxNode().IsMultiLine.ShouldBeFalse();
        }

        [Fact]
        public void ShouldGenerateCode()
        {
            new TypeOfSyntaxNode { Value = new ValueSyntaxNode { TypeName = "bool?" } }
                .GenerateCode()
                .ShouldBe("typeof(bool?)");
        }
    }
}