using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Validation
{
    public class DictionaryTests
    {
        [Theory, MemberData(nameof(GetShouldPassWhenTypesMatchData))]
        public void ShouldPassWhenTypesMatch(Type type, object dataContract)
        {
            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
        }

        public static IEnumerable<object[]> GetShouldPassWhenTypesMatchData()
        {
            var types = new[]
            {
                typeof(IDictionary<int, int>),
                typeof(Dictionary<int, int>)
            };

            return from type in types
                   from dataContract in GetDataContracts()
                   select new[] { type, dataContract };
        }

        private static IEnumerable<object> GetDataContracts()
        {
            return new object[]
            {
                typeof(Dictionary<int, int>),
                new Dictionary<int,int>(),
                new Dictionary<int>(typeof(int)),
            };
        }

        [Theory, MemberData(nameof(GetShouldFailWhenTypesDoNotMatchData))]
        public void ShouldFailWhenTypesDoNotMatch(object dataContract)
        {
            var type = typeof(int);

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "type mismatch, expected 'dictionary' but found 'int'." });
        }

        public static IEnumerable<object[]> GetShouldFailWhenTypesDoNotMatchData()
        {
            return GetDataContracts().Select(dataContract => new[] { dataContract });
        }

        [Fact]
        public void ShouldFailWhenKeyIsInvalid()
        {
            var type = typeof(Dictionary<string, int>);
            var dataContract = typeof(Dictionary<int, int>);

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "Key : type mismatch, expected 'int' but found 'string'." });
        }

        [Fact]
        public void ShouldFailWhenValueIsInvalid()
        {
            var type = typeof(Dictionary<int, string>);
            var dataContract = typeof(Dictionary<int, int>);

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "Value : type mismatch, expected 'int' but found 'string'." });
        }
    }
}