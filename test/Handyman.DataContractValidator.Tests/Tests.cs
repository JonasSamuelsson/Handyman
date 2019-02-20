using Shouldly;
using System.Collections.Generic;
using Xunit;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace Handyman.DataContractValidator.Tests
{
    public class Tests
    {
        [Fact]
        public void ShouldValidateAgainstTypeContract()
        {
            var type = typeof(Actual.Root);
            var dataContract = typeof(Expected.Root);

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[]
            {
                "Child.Boolean : unexpected property.",
                "Child.Collection.Item.Number : type mismatch, expected int but found long.",
                "Child.Dictionary.Key : type mismatch, expected int but found long.",
                "Child.Dictionary.Value.Number : type mismatch, expected int but found long.",
                "Child.Number : type mismatch, expected int but found long.",
                "Child.Text : expected property not found.",
                "Number : type mismatch, expected int but found long."
            });
        }

        [Fact]
        public void ShouldValidateAgainstInlineDefinedContract()
        {
            var type = typeof(Actual.Root);

            var dataContract = new
            {
                Child = new
                {
                    Collection = new[] { new { Number = new Value<int>() } },
                    Dictionary = new Dictionary<int>(new { Number = new Value<int>() }),
                    Number = new Value<int>(),
                    Text = new Value<string>()
                },
                Number = new Value<int>(),
            };

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[]
            {
                "Child.Boolean : unexpected property.",
                "Child.Collection.Item.Number : type mismatch, expected int but found long.",
                "Child.Dictionary.Key : type mismatch, expected int but found long.",
                "Child.Dictionary.Value.Number : type mismatch, expected int but found long.",
                "Child.Number : type mismatch, expected int but found long.",
                "Child.Text : expected property not found.",
                "Number : type mismatch, expected int but found long."
            });
        }

        private class Actual
        {
            internal class Root
            {
                public Child Child { get; set; }
                public long Number { get; set; }
            }

            internal class Child
            {
                public bool Boolean { get; set; }
                public IEnumerable<Thing> Collection { get; set; }
                public Dictionary<long, Thing> Dictionary { get; set; }
                public long Number { get; set; }
            }

            internal class Thing
            {
                public long Number { get; set; }
            }
        }

        private class Expected
        {
            internal class Root
            {
                public Child Child { get; set; }
                public int Number { get; set; }
            }

            internal class Child
            {
                public IEnumerable<Thing> Collection { get; set; }
                public Dictionary<int, Thing> Dictionary { get; set; }
                public int Number { get; set; }
                public string Text { get; set; }
            }

            internal class Thing
            {
                public int Number { get; set; }
            }
        }
    }
}
