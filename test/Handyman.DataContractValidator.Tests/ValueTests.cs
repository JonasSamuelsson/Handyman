using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class ValueTests
    {
        [Theory, MemberData(nameof(GetDataContracts))]
        public void ShouldPassWhenTypesMatch(object dataContract)
        {
            var type = typeof(int);

            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
        }

        [Theory, MemberData(nameof(GetDataContracts))]
        public void ShouldFailWhenTypesDoNotMatch(object dataContract)
        {
            var type = typeof(int?);
            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "type mismatch, expected int but found int?." });
        }

        public static IEnumerable<object[]> GetDataContracts()
        {
            yield return new object[] { typeof(int) };
            yield return new object[] { new Value<int>() };
        }
    }
}