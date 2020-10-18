using Handyman.Tools.Docs.ImportCode;
using Handyman.Tools.Docs.Utils.Deprecated;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.Tools.Docs.Tests
{
    public class ImportCodeElementAttributesParserTests
    {
        [Fact]
        public void ShouldRequireSrc()
        {
            var attributes = new List<Attribute>();
            var errors = new List<string>();

            new ImportCodeElementAttributesParser().Validate(attributes, errors).ShouldBeFalse();

            attributes.Add(new Attribute { Name = "src", Value = @"x:\" });

            new ImportCodeElementAttributesParser().Validate(attributes, errors).ShouldBeTrue();
        }

        [Fact]
        public void ShouldFailIfBothIdAndLinesAreProvided()
        {
            var attributes = new List<Attribute>
            {
                new Attribute {Name = "src", Value = @"x:\"},
                new Attribute {Name = "id", Value = "test"},
                new Attribute {Name = "lines", Value = "1"}
            };
            var errors = new List<string>();

            new ImportCodeElementAttributesParser().Validate(attributes, errors).ShouldBeFalse();
        }

        [Theory]
        [InlineData("test", true)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        public void IdCanNotBeEmpty(string value, bool expected)
        {
            var attributes = new List<Attribute>
            {
                new Attribute {Name = "src", Value = @"x:\"},
                new Attribute {Name = "id", Value = value}
            };
            var errors = new List<string>();

            new ImportCodeElementAttributesParser().Validate(attributes, errors).ShouldBe(expected);
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("1-3", true)]
        [InlineData("3-1", false)]
        [InlineData("x", false)]
        [InlineData("1-", false)]
        public void LinesShouldHaveValidFormat(string value, bool expected)
        {
            var attributes = new List<Attribute>
            {
                new Attribute {Name = "src", Value = @"x:\"},
                new Attribute {Name = "lines", Value = value}
            };
            var errors = new List<string>();

            new ImportCodeElementAttributesParser().Validate(attributes, errors).ShouldBe(expected);
        }
    }
}