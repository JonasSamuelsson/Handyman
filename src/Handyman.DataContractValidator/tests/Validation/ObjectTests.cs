using Shouldly;
using System;
using System.Collections.Generic;
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

        [Theory, MemberData(nameof(ShouldValidateNullableReferenceTypesParams))]
        public void ShouldAccountForNullableAnnotations(Type type, object dataContract, string expected)
        {
            var validator = new DataContractValidator();

            if (expected == "pass")
            {
                validator.Validate(type, dataContract);
            }
            else
            {
                Should.Throw<ValidationException>(() => validator.Validate(type, dataContract))
                    .Message.ShouldBe(expected);
            }
        }

        public static IEnumerable<object[]> ShouldValidateNullableReferenceTypesParams()
        {
            yield return new object[]
            {
                typeof(NullableEnabledWithNullable),
                new
                {
                    Text = Nullable.String
                },
                "pass"
            };

            yield return new object[]
            {
                typeof(NullableEnabledWithNullable),
                new
                {
                    Text = typeof(string)
                },
                "Text : type mismatch, expected 'string' but found 'string?'."
            };

            yield return new object[]
            {
                typeof(NullableEnabledWithoutNullable),
                new
                {
                    Text = typeof(string)
                },
                "pass"
            };

            yield return new object[]
            {
                typeof(NullableEnabledWithoutNullable),
                new
                {
                    Text = Nullable.String
                },
                "Text : type mismatch, expected 'string?' but found 'string'."
            };

            yield return new object[]
            {
                typeof(NullableDisabled),
                new
                {
                    Text = typeof(string)
                },
                "pass"
            };

            yield return new object[]
            {
                typeof(NullableDisabled),
                new
                {
                    Text = Nullable.String
                },
                "Text : type mismatch, expected 'string?' but found 'string'."
            };
        }

#nullable enable

        private class NullableEnabledWithNullable
        {
            public string? Text { get; set; }
        }

        private class NullableEnabledWithoutNullable
        {
            public string Text { get; set; }
        }

#nullable disable

        private class NullableDisabled
        {
            public string Text { get; set; }
        }

#nullable restore
    }
}