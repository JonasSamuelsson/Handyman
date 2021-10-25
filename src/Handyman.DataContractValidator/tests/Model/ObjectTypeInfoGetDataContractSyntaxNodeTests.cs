using Handyman.DataContractValidator.CodeGen.DataContracts;
using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.Tests.CodeGen;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Model
{
    public class ObjectTypeInfoGetDataContractSyntaxNodeTests
    {
        [Fact]
        public void ShouldGetSyntaxNode()
        {
            var valueSyntaxNode = new TestSyntaxNode();

            var objectSyntaxNode = new ObjectTypeInfo
            {
                Properties = new[]
                    {
                        new PropertyInfo
                        {
                            Name = "Foo",
                            Value = new TestTypeInfo
                            {
                                DataContractSyntaxNode = valueSyntaxNode
                            }
                        }
                    }
            }.GetDataContractSyntaxNode()
                .ShouldBeOfType<ObjectSyntaxNode>();

            var property = objectSyntaxNode.Properties.Single();

            property.Name.ShouldBe("Foo");
            property.Value.ShouldBe(valueSyntaxNode);
        }

        [Fact]
        public void ShouldWrapNullableReferences()
        {
            var valueSyntaxNode = new TestSyntaxNode();

            new ObjectTypeInfo
            {
                Properties = new[]
                    {
                        new PropertyInfo
                        {
                            Name = "Foo",
                            Value = new TestTypeInfo
                            {
                                IsNullable = true,
                                IsReference = true,
                                DataContractSyntaxNode = valueSyntaxNode
                            }
                        }
                    }
            }.GetDataContractSyntaxNode()
                .ShouldBeOfType<ObjectSyntaxNode>()
                .Properties.Single().Value.ShouldBeOfType<CanBeNullSyntaxNode>()
                .Value.ShouldBe(valueSyntaxNode);
        }

        [Fact]
        public void ShouldNotIncludeIgnoredProperties()
        {
            new ObjectTypeInfo
            {
                Properties = new[]
                    {
                        new PropertyInfo
                        {
                            IsIgnored = true,
                            Name = "Foo",
                            Value = new AnyTypeInfo()
                        }
                    }
            }.GetDataContractSyntaxNode()
                .ShouldBeOfType<ObjectSyntaxNode>()
                .Properties.ShouldBeEmpty();
        }
    }
}