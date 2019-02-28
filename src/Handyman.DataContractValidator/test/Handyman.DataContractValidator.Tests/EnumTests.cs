using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class EnumTests
    {
        [Fact]
        public void ShouldPassWhenTypesMatch()
        {
            var type = typeof(Number);
            var dataContract = new Enum(0, 1);

            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
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

        private enum Number
        {
            Zero = 0,
            One = 1
        }
    }
}