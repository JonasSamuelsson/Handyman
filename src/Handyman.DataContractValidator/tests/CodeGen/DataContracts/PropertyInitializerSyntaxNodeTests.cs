using Handyman.DataContractValidator.CodeGen.DataContracts;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen.DataContracts
{
    public class PropertyInitializerSyntaxNodeTests
    {
        [Fact]
        public void ShouldDelegateIsMultiLineToValue()
        {
            var values = new[] { false, true };

            foreach (var value in values)
            {
                new PropertyInitializerSyntaxNode
                {
                    Value = new TestSyntaxNode
                    {
                        IsMultiLine = value
                    }
                }.IsMultiLine.ShouldBe(value);
            }
        }

        [Fact]
        public void ShouldGenerateCode()
        {
            new PropertyInitializerSyntaxNode
            {
                Name = "foo",
                Value = new TestSyntaxNode
                {
                    Code = "bar"
                }
            }
                .GenerateCode()
                .ShouldBe("foo = bar");
        }
    }
}