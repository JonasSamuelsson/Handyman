using Handyman.AspNetCore.ApiVersioning.MajorMinorPreReleaseVersionScheme;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning.MajorMinorPreReleaseVersionScheme
{
    public class MajorMinorPreReleaseApiVersionParserTests
    {
        [Theory]
        [InlineData("1")]
        [InlineData("1-alpha")]
        [InlineData("1.0")]
        [InlineData("1.0-alpha")]
        [InlineData("1.123")]
        [InlineData("1.123-alpha")]
        public void ShouldParse(string version)
        {
            new MajorMinorPreReleaseApiVersionParser().TryParse(version, out var apiVersion).ShouldBeTrue();
            apiVersion.Text.ShouldBe(version);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("x")]
        [InlineData("1x")]
        [InlineData("1.")]
        [InlineData("1.2.3")]
        [InlineData("1-")]
        [InlineData("1-?")]
        [InlineData("1-alpha-beta")]
        public void ShouldNotParse(string candidate)
        {
            new MajorMinorPreReleaseApiVersionParser().TryParse(candidate, out _).ShouldBeFalse();
        }
    }
}