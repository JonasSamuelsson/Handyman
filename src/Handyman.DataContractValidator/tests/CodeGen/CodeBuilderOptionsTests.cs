using Handyman.DataContractValidator.CodeGen;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen
{
    public class CodeBuilderOptionsTests
    {
        [Fact]
        public void ShouldHaveDefaultIndentation()
        {
            new CodeBuilderOptions().Indentation.ShouldBe("   ");
        }
    }
}