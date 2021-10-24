using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.TypeInfoResolvers;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.TypeInfoResolvers
{
    public class CollectionTypeInfoResolverTests
    {
        [Theory, MemberData(nameof(ShouldResolveCollectionTypeInfosParams))]
        public void ShouldResolveCollectionTypeInfos(object o, Type itemType)
        {
            new TypeInfoResolverContext().GetTypeInfo(o).ShouldBeOfType<CollectionTypeInfo>()
                .Item.ShouldBeOfType(itemType);
        }

        public static IEnumerable<object[]> ShouldResolveCollectionTypeInfosParams()
        {
            return new[]
            {
                new object[] { typeof(IEnumerable<string>), typeof(ValueTypeInfo) },
                new object[] { new[] { typeof(string) }, typeof(ValueTypeInfo) }
            };
        }
    }
}