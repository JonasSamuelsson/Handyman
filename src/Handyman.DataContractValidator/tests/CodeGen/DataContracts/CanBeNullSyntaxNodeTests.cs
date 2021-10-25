using Handyman.DataContractValidator.CodeGen.DataContracts;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen.DataContracts
{
    public class CanBeNullSyntaxNodeTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ShouldGenerateCode(bool valueIsMultiLine)
        {
            new CanBeNullSyntaxNode
            {
                Value = new TestSyntaxNode
                {
                    IsMultiLine = valueIsMultiLine,
                    Code = "foo"
                }
            }
                .GenerateCode()
                .ShouldBe("new CanBeNull(foo)");
        }
    }
}