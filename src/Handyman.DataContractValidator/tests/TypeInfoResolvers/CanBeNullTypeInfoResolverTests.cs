using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.TypeInfoResolvers;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.DataContractValidator.Tests.TypeInfoResolvers
{
    public class CanBeNullTypeInfoResolverTests
    {
        [Fact]
        public void ShouldResolveTypeWithNullabilityDisabled()
        {
            var type = typeof(NullableDisabled);

            var typeInfo = new TypeInfoResolverContext().GetTypeInfo(type)
                .ShouldBeOfType<ObjectTypeInfo>();

            typeInfo.Properties.Single(x => x.Name == "Text").Value.IsNullable.ShouldBeNull();
        }

        [Fact]
        public void ShouldResolveTypeWithNullabilityEnabled()
        {
            var type = typeof(NullableEnabled);

            var typeInfo = new TypeInfoResolverContext().GetTypeInfo(type)
                .ShouldBeOfType<ObjectTypeInfo>();

            typeInfo.Properties.Single(x => x.Name == "NotNullable").Value.IsNullable.ShouldBe(false);
            typeInfo.Properties.Single(x => x.Name == "Nullable").Value.IsNullable.ShouldBe(true);
        }

        [Fact]
        public void ShouldResolveDataContract()
        {
            var dataContract = new
            {
                NotNullable = typeof(string),
                Nullable = CanBeNull.String
            };

            var typeInfo = new TypeInfoResolverContext().GetTypeInfo(dataContract)
                .ShouldBeOfType<ObjectTypeInfo>();

            typeInfo.Properties.Single(x => x.Name == "NotNullable").Value.IsNullable.ShouldBeNull();
            typeInfo.Properties.Single(x => x.Name == "Nullable").Value.IsNullable.ShouldBe(true);
        }

#nullable enable

        public class NullableEnabled
        {
            public string NotNullable { get; set; } = null!;
            public string? Nullable { get; set; }
        }

#nullable disable

        public class NullableDisabled
        {
            public string Text { get; set; }
        }
    }
}