using Shouldly;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class CollectionTests
    {
        [Theory]
        [MemberData(nameof(GetShouldPassWhenTypesMatchData))]
        public void ShouldPassWhenTypesMatch(Type type, object dataContract)
        {
            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
        }

        public static IEnumerable<object[]> GetShouldPassWhenTypesMatchData()
        {
            var types = new[]
            {
                typeof(int[]),
                typeof(IEnumerable<int>),
                typeof(List<int>)
            };

            var dataContracts = GetDataContracts();

            return from type in types
                   from dataContract in dataContracts
                   select new[] { type, dataContract };
        }

        private static object[] GetDataContracts()
        {
            return new object[]
            {
                typeof(IEnumerable<int>),
                new[] {typeof(int)},
                new int[] { },
                new Collection<int>(),
                new List<int>()
            };
        }

        [Theory]
        [MemberData(nameof(GetShouldFailWhenTypesDoNotMatchData))]
        public void ShouldFailWhenTypesDoNotMatch(object dataContract)
        {
            var type = typeof(int);

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "type mismatch, expected enumerable but found int." });
        }

        public static IEnumerable<object[]> GetShouldFailWhenTypesDoNotMatchData()
        {
            return GetDataContracts().Select(x => new[] { x });
        }
    }
}