using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class EnumTests
    {
        [Theory, MemberData(nameof(GetShouldPassWhenTypesMatchParams))]
        public void ShouldPassWhenTypesMatch(Type type, object dataContract)
        {
            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
        }

        public static IEnumerable<object[]> GetShouldPassWhenTypesMatchParams()
        {
            yield return new object[] { typeof(Flags), new Enum(EnumKind.Flags, new[] { 0, 1, 2 }) };
            yield return new object[] { typeof(Flags?), new Enum(EnumKind.Flags | EnumKind.Nullable, new[] { 0, 1, 2 }) };
            yield return new object[] { typeof(Number), new Enum(0, 1) };
            yield return new object[] { typeof(Number?), new Enum(EnumKind.Nullable, new[] { 0, 1 }) };
        }

        [Fact]
        public void ShouldFailWhenTypesDoNotMatch()
        {
            var type = typeof(int);
            var dataContract = new Enum(0);

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "type mismatch, expected enum but found int." });
        }

        [Fact]
        public void ShouldFailWhenEnumIsMissingExpectedValue()
        {
            var type = typeof(Number);
            var dataContract = new Enum(0, 1, 2);

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "enum values mismatch, expected [0,1,2] but found [0,1]." });
        }

        [Fact]
        public void ShouldFailWhenEnumHasUnexpectedValue()
        {
            var type = typeof(Number);
            var dataContract = new Enum(0);

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "enum values mismatch, expected [0] but found [0,1]." });
        }

        [Flags]
        private enum Flags
        {
            None = 0,
            One = 1,
            Two = 2
        }

        private enum Number
        {
            Zero = 0,
            One = 1
        }
    }
}