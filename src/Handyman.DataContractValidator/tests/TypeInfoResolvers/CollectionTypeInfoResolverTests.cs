using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.TypeInfoResolvers;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.TypeInfoResolvers
{
    public class CollectionTypeInfoResolverTests
    {
        [Theory, MemberData(nameof(ShouldResolveCollectionTypeInfosParams))]
        public void ShouldResolveCollectionTypeInfos(object o, string typeName)
        {
            new TypeInfoResolverContext().GetTypeInfo(o).ShouldBeOfType<CollectionTypeInfo>().Item.TypeName.ShouldBe(typeName);
        }

        public static IEnumerable<object[]> ShouldResolveCollectionTypeInfosParams()
        {
            return new[]
            {
                new object[] { typeof(IEnumerable<int>), "int" },
                new object[] { new[] { typeof(int) }, "int" },
                new object[] { new[] { new { } }, "object" }
            };
        }
    }
}