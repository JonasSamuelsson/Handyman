using Handyman.AspNetCore.ApiVersioning.MajorMinorPreReleaseVersionScheme;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning.MajorMinorPreReleaseVersionScheme
{
    public class MajorMinorPreReleaseApiVersionParserTests
    {
        [Theory]
        [InlineData("1", "1.0")]
        [InlineData("1-alpha", "1.0-alpha")]
        [InlineData("1.0", "1.0")]
        [InlineData("1.1", "1.1")]
        [InlineData("1.1-alpha", "1.1-alpha")]
        public void ShouldParse(string candidate, string text)
        {
            new MajorMinorPreReleaseApiVersionParser().TryParse(candidate, out var apiVersion).ShouldBeTrue();
            apiVersion.Text.ShouldBe(text);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("x")]
        [InlineData("1x")]
        [InlineData("1.")]
        [InlineData("1.2.3")]
        [InlineData("1-")]
        public void ShouldNotParse(string candidate)
        {
            new MajorMinorPreReleaseApiVersionParser().TryParse(candidate, out _).ShouldBeFalse();
        }
    }
}