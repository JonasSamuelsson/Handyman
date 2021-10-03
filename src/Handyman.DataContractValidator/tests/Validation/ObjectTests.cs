using Shouldly;
using System;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Validation
{
    public class ObjectTests
    {
        [Fact]
        public void ShouldPassWhenTypesMatch()
        {
            var type = typeof(ClassWithIntNumberProperty);
            var dataContract = new { Number = typeof(int) };

            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
        }

        [Fact]
        public void ShouldFailWhenTypesIsMissingExpectedProperty()
        {
            var type = typeof(object);
            var dataContract = new { Number = typeof(int) };

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "Number : expected property not found." });
        }

        [Fact]
        public void ShouldFailWhenTypesHasUnexpectedProperty()
        {
            var type = typeof(ClassWithIntNumberProperty);
            var dataContract = new { };

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "Number : unexpected property." });
        }

        [Fact]
        public void ShouldIgnorePropertiesDecoratedWithIgnoreAttribute()
        {
            var type = typeof(TypeWithIgnoredProperty);
            var dataContract = new { };

            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
        }

        [Fact]
        public void ShouldFailWhenExpectedPropertyIsDecoratedWithIgnoreAttribute()
        {
            var type = typeof(TypeWithIgnoredProperty);
            var dataContract = new { Number = typeof(int) };

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "Number : expected property found but it is decorated with ignore attribute." });
        }

        private class ClassWithIntNumberProperty
        {
            public int Number { get; set; }
        }

        private class TypeWithIgnoredProperty
        {
            [AttributeWithIgnoreInTheName]
            public int Number { get; set; }
        }

        private class AttributeWithIgnoreInTheNameAttribute : Attribute { }
    }
}