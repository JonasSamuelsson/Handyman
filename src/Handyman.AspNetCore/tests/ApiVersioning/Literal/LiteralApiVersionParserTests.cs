using Handyman.AspNetCore.ApiVersioning.Schemes.Literal;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning.Literal
{
    public class LiteralApiVersionParserTests
    {
        [Theory]
        [InlineData("1")]
        [InlineData("1.2.3")]
        [InlineData("foobar")]
        public void ShouldParse(string candidate)
        {
            new LiteralApiVersionParser().TryParse(candidate, out var apiVersion).ShouldBeTrue();
            apiVersion.Text.ShouldBe(candidate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldNotParse(string candidate)
        {
            new LiteralApiVersionParser().TryParse(candidate, out _).ShouldBeFalse();
        }
    }
}