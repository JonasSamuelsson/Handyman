using Handyman.DataContractValidator.CodeGen.DataContracts;
using Handyman.DataContractValidator.Model;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Model
{
    public class AnyTypeInfoTests
    {
        [Fact]
        public void ShouldNotBePrimitive()
        {
            new AnyTypeInfo().IsPrimitive.ShouldBeFalse();
        }

        [Fact]
        public void ShouldBeReference()
        {
            new AnyTypeInfo().IsReference.ShouldBeTrue();
        }

        [Fact]
        public void ShouldProvideTypeName()
        {
            new AnyTypeInfo().TypeName.ShouldBe("any");
        }

        [Fact]
        public void ShouldGetDataContractSyntaxNode()
        {
            new AnyTypeInfo().GetDataContractSyntaxNode()
                .ShouldBeOfType<AnySyntaxNode>();
        }
    }
}