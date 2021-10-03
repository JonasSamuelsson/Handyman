using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Validation
{
    public class ValueTests
    {
        [Fact]
        public void ShouldPassWhenTypesMatch()
        {
            var type = typeof(int);
            var dataContract = typeof(int);

            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
        }

        [Theory, MemberData(nameof(ShouldFailWhenTypesDoNotMatchParams))]
        public void ShouldFailWhenTypesDoNotMatch(Type type, object dataContract, string error)
        {
            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { error });
        }

        public static IEnumerable<object[]> ShouldFailWhenTypesDoNotMatchParams()
        {
            yield return new object[] { typeof(int), typeof(long), "type mismatch, expected 'long' but found 'int'." };
            yield return new object[] { typeof(int), typeof(int?), "type mismatch, expected 'int?' but found 'int'." };
        }
    }
}