using Handyman.AspNetCore.ApiVersioning.MajorMinorPreReleaseVersionScheme;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning.MajorMinorPreReleaseVersionScheme
{
    public class MajorMinorPreReleaseApiVersionTests
    {
        [Theory]
        [InlineData("1", "1", true)]
        [InlineData("1", "1.0", true)]
        [InlineData("1.0", "1.0", true)]
        [InlineData("1-alpha", "1.0-ALPHA", true)]
        [InlineData("1", "1-alpha", false)]
        [InlineData("1", "1.1", false)]
        [InlineData("1", "2", false)]
        public void ShouldCompareVersions(string a, string b, bool result)
        {
            var parser = new MajorMinorPreReleaseApiVersionParser();
            parser.TryParse(a, out var x).ShouldBeTrue();
            parser.TryParse(b, out var y).ShouldBeTrue();
            x.IsMatch(y).ShouldBe(result);
        }
    }
}