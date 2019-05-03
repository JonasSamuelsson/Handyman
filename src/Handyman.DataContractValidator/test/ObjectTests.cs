using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class ObjectTests
    {
        [Fact]
        public void ShouldPassWhenTypesMatch()
        {
            var type = new { Number = default(int), Text = default(string) }.GetType();
            var dataContract = new { Number = typeof(int), Text = typeof(string) };

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
            var type = new { Number = default(int) }.GetType();
            var dataContract = new { };

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "Number : unexpected property." });
        }

        [Fact]
        public void ShouldIgnorePropertiesDecoratedWithJsonIgnoreAttribute()
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

            errors.ShouldBe(new[] { "Number : expected property found but decorated with ignore attribute." });

        }

        private class TypeWithIgnoredProperty
        {
            [JsonIgnore]
            public int Number { get; set; }
        }

        [Fact]
        public void ShouldPassWhenRecursiveTypesMatch()
        {
            var type = typeof(ActualRecursiveType);
            var dataContract = typeof(ExpectedRecursiveType);

            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
        }

        [Fact]
        public void ShouldFailWhenRecursiveTypesDoNotMatch()
        {
            var type = typeof(ActualRecursiveType);
            var dataContract = new
            {
                Text = typeof(string),
                Child = new
                {
                    Text = typeof(string),
                    Child = new
                    {
                        Text = typeof(string),
                        Child = new
                        {
                            Text = typeof(string)
                        }
                    }
                }
            };

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "Child.Child.Child.Child : unexpected property." });
        }

        private class ActualRecursiveType
        {
            public string Text { get; set; }
            public ActualRecursiveType Child { get; set; }
        }

        private class ExpectedRecursiveType
        {
            public string Text { get; set; }
            public ExpectedRecursiveType Child { get; set; }
        }
    }
}