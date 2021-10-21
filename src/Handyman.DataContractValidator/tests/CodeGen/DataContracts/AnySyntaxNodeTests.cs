using Handyman.DataContractValidator.CodeGen.DataContracts;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen.DataContracts
{
    public class AnySyntaxNodeTests
    {
        [Fact]
        public void ShouldBeSingleLine()
        {
            new AnySyntaxNode().IsMultiLine.ShouldBeFalse();
        }

        [Fact]
        public void ShouldGenerateCode()
        {
            new AnySyntaxNode()
                .GenerateCode()
                .ShouldBe("new Any()");
        }
    }
}