using Handyman.DataContractValidator.CodeGen.DataContracts;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen.DataContracts
{
    public class CollectionSyntaxNodeTests
    {
        [Fact]
        public void ShouldDelegateIsMultiLineToPropertyValue()
        {
            var values = new[] { false, true };

            foreach (var value in values)
            {
                new CollectionSyntaxNode
                {
                    Item = new TestSyntaxNode
                    {
                        IsMultiLine = value
                    }
                }.IsMultiLine.ShouldBe(value);
            }
        }

        [Fact]
        public void ShouldGenerateCodeForSingleLineItem()
        {
            new CollectionSyntaxNode
            {
                Item = new TestSyntaxNode
                {
                    Code = "foo",
                    IsMultiLine = false
                }
            }
                .GenerateCode()
                .ShouldBe("new [] { foo }");
        }

        [Fact]
        public void ShouldGenerateCodeForMultiLineItem()
        {
            new CollectionSyntaxNode
            {
                Item = new TestSyntaxNode
                {
                    Code = "foo",
                    IsMultiLine = true
                }
            }
                .GenerateCode()
                .ShouldBe(@"new []
{
   foo
}");
        }
    }
}