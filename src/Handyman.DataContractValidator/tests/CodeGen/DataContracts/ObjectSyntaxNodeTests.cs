using Handyman.DataContractValidator.CodeGen.DataContracts;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen.DataContracts
{
    public class ObjectSyntaxNodeTests
    {
        [Fact]
        public void ShouldBeMultiLineIfObjectHasProperties()
        {
            new ObjectSyntaxNode
            {
                Properties = new PropertyInitializerSyntaxNode[] { null! }
            }.IsMultiLine.ShouldBeTrue();
        }

        [Fact]
        public void ShouldBeSingleLineIfObjectDoesNotHaveProperties()
        {
            new ObjectSyntaxNode
            {
                Properties = Enumerable.Empty<PropertyInitializerSyntaxNode>()
            }.IsMultiLine.ShouldBeFalse();
        }

        [Fact]
        public void ShouldGenerateCodeForObjectWithoutProperties()
        {
            new ObjectSyntaxNode
            {
                Properties = Enumerable.Empty<PropertyInitializerSyntaxNode>()
            }
                .GenerateCode()
                .ShouldBe("new { }");
        }

        [Fact]
        public void ShouldGenerateCodeForObjectWithSingleProperty()
        {
            new ObjectSyntaxNode
            {
                Properties = new[]
                {
                    new PropertyInitializerSyntaxNode
                    {
                        Name = "Foo",
                        Value = new TestSyntaxNode
                        {
                            Code = "value",
                            IsMultiLine = false
                        }
                    }
                }
            }
                .GenerateCode()
                .ShouldBe(@"new
{
   Foo = value
}");
        }

        [Fact]
        public void ShouldGenerateCodeForObjectWithMultipleProperties()
        {
            new ObjectSyntaxNode
            {
                Properties = new[]
                    {
                        new PropertyInitializerSyntaxNode
                        {
                            Name = "Foo",
                            Value = new TestSyntaxNode
                            {
                                Code = "one",
                                IsMultiLine = false
                            }
                        },
                        new PropertyInitializerSyntaxNode
                        {
                            Name = "Bar",
                            Value = new TestSyntaxNode
                            {
                                Code = "two",
                                IsMultiLine = false
                            }
                        }
                    }
            }
                .GenerateCode()
                .ShouldBe(@"new
{
   Foo = one,
   Bar = two
}");
        }
    }
}