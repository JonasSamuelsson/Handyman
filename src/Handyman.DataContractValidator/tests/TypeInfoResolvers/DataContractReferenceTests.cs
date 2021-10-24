using System.Linq;
using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.TypeInfoResolvers;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests.TypeInfoResolvers
{
    public class DataContractReferenceTests
    {
        [Fact]
        public void ShouldResolveDataContractReference()
        {
            var store = new DataContractStore();

            store.Add("x", typeof(string));

            var dataContract = new
            {
                Foo = store.Get("x")
            };

            var typeInfo = new TypeInfoResolverContext().GetTypeInfo(dataContract);

            typeInfo.ShouldBeOfType<ObjectTypeInfo>()
                .Properties.Single().Value.ShouldBeOfType<ValueTypeInfo>()
                .Type.ShouldBe(typeof(string));
        }
    }
}