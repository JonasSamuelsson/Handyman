using Handyman.DataContractValidator.CodeGen.DataContracts;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen.DataContracts
{
    public class DictionarySyntaxNodeTests
    {
        [Fact]
        public void ShouldDelegateIsMultiLineToValue()
        {
            var values = new[] { false, true };

            foreach (var value in values)
            {
                new DictionarySyntaxNode
                {
                    Value = new TestSyntaxNode { IsMultiLine = value }
                }.IsMultiLine.ShouldBe(value);
            }
        }

        [Fact]
        public void ShouldGenerateCodeForMultiLineValue()
        {
            new DictionarySyntaxNode
            {
                Key = new ValueSyntaxNode
                {
                    TypeName = "int"
                },
                Value = new TestSyntaxNode
                {
                    Code = "typeof(string)",
                    IsMultiLine = true
                }
            }
                .GenerateCode()
                .ShouldBe(@"new Dictionary<int>
{
   typeof(string)
}");
        }

        [Fact]
        public void ShouldGenerateCodeForSingleLineValue()
        {
            new DictionarySyntaxNode
            {
                Key = new ValueSyntaxNode
                {
                    TypeName = "int"
                },
                Value = new TestSyntaxNode
                {
                    Code = "typeof(string)",
                    IsMultiLine = false
                }
            }
                .GenerateCode()
                .ShouldBe("new Dictionary<int> { typeof(string) }");
        }
    }
}