using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.TypeInfoResolvers;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.TypeInfoResolvers
{
    public class AnyTypeInfoResolverTests
    {
        [Theory, MemberData(nameof(ShouldResolveAnyTypeInfosParams))]
        public void ShouldResolveAnyTypeInfos(object o)
        {
            new TypeInfoResolverContext().GetTypeInfo(o).ShouldBeOfType<AnyTypeInfo>();
        }

        public static IEnumerable<object[]> ShouldResolveAnyTypeInfosParams()
        {
            return new[]
            {
                new object[] { new Any() },
                new object[] { typeof(Any) }
            };
        }
    }
}