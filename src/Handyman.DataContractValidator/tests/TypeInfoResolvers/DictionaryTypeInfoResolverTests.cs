using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.TypeInfoResolvers;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.TypeInfoResolvers
{
    public class DictionaryTypeInfoResolverTests
    {
        [Theory, MemberData(nameof(ShouldResolveEnumTypeInfosParams))]
        public void ShouldResolveDictionaryTypeInfos(object o, string keyTypeName, string valueTypeName)
        {
            var typeInfo = new TypeInfoResolverContext().GetTypeInfo(o).ShouldBeOfType<DictionaryTypeInfo>();
            typeInfo.Key.TypeName.ShouldBe(keyTypeName);
            typeInfo.Value.TypeName.ShouldBe(valueTypeName);
        }

        public static IEnumerable<object[]> ShouldResolveEnumTypeInfosParams()
        {
            return new[]
            {
                new object[] { typeof(IDictionary<int, string>), "int", "string" },
                new object[] { typeof(Dictionary<int, string>), "int", "string" },
                new object[] { new Dictionary<int>(typeof(string)), "int", "string" },
                new object[] { new Dictionary<int>(new { }), "int", "object" }
            };
        }
    }
}