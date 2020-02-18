using Handyman.AspNetCore.ApiVersioning.Schemes.Literal;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning.Literal
{
    public class LiteralApiVersionTests
    {
        [Theory]
        [InlineData("1", "1", true)]
        [InlineData("1-alpha", "1-alpha", true)]
        [InlineData("alpha", "alpha", true)]
        [InlineData("1", "1.0", false)]
        [InlineData("foo", "FOO", false)]
        public void ShouldCompare(string a, string b, bool result)
        {
            var parser = new LiteralApiVersionParser();
            parser.TryParse(a, out var x).ShouldBeTrue();
            parser.TryParse(b, out var y).ShouldBeTrue();
            x.IsMatch(y).ShouldBe(result);
        }
    }
}