﻿using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.TypeInfoResolvers;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.DataContractValidator.Tests.TypeInfoResolvers
{
    public class NullableTypeInfoResolverTests
    {
        [Fact]
        public void ShouldResolveTypeWithNullabilityDisabled()
        {
            var type = typeof(NullableDisabled);

            var typeInfo = new TypeInfoResolverContext().GetTypeInfo(type)
                .ShouldBeOfType<ObjectTypeInfo>();

            typeInfo.Properties.Single(x => x.Name == "Text").Type.IsNullable.ShouldBeNull();
        }

        [Fact]
        public void ShouldResolveTypeWithNullabilityEnabled()
        {
            var type = typeof(NullableEnabled);

            var typeInfo = new TypeInfoResolverContext().GetTypeInfo(type)
                .ShouldBeOfType<ObjectTypeInfo>();

            typeInfo.Properties.Single(x => x.Name == "NotNullable").Type.IsNullable.ShouldBe(false);
            typeInfo.Properties.Single(x => x.Name == "Nullable").Type.IsNullable.ShouldBe(true);
        }

        [Fact]
        public void ShouldResolveDataContract()
        {
            var dataContract = new
            {
                NotNullable = typeof(string),
                Nullable = Nullable.String
            };

            var typeInfo = new TypeInfoResolverContext().GetTypeInfo(dataContract)
                .ShouldBeOfType<ObjectTypeInfo>();

            typeInfo.Properties.Single(x => x.Name == "NotNullable").Type.IsNullable.ShouldBeNull();
            typeInfo.Properties.Single(x => x.Name == "Nullable").Type.IsNullable.ShouldBe(true);
        }

#nullable enable

        public class NullableEnabled
        {
            public string NotNullable { get; set; }
            public string? Nullable { get; set; }
        }

#nullable disable

        public class NullableDisabled
        {
            public string Text { get; set; }
        }
    }
}