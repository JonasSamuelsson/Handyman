using Handyman.DataContractValidator.CodeGen.DataContracts;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen.DataContracts
{
    public class NullableSyntaxNodeTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ShouldGenerateCode(bool valueIsMultiLine)
        {
            new NullableSyntaxNode
            {
                Value = new TestSyntaxNode
                {
                    IsMultiLine = valueIsMultiLine,
                    Code = "foo"
                }
            }
                .GenerateCode()
                .ShouldBe("new Nullable(foo)");
        }
    }
}