using Handyman.AspNetCore.ApiVersioning.SemVer;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning.SemVer
{
    public class SemVerApiVersionParserTests
    {
        [Theory, MemberData(nameof(GetShouldParseParams))]
        internal void ShouldParse(string candidate, string text, int major, int minor, object[] preReleaseLabels)
        {
            new SemVerApiVersionParser().TryParse(candidate, out var version).ShouldBeTrue();
            version.ShouldBeOfType<SemVerApiVersion>();
            ((SemVerApiVersion)version).Text.ShouldBe(text);
            ((SemVerApiVersion)version).Major.ShouldBe(major);
            ((SemVerApiVersion)version).Minor.ShouldBe(minor);
            ((SemVerApiVersion)version).PreReleaseLabels.ShouldBe(preReleaseLabels);
        }

        public static IEnumerable<object[]> GetShouldParseParams()
        {
            return new[]
            {
                new object[] {"1", "1.0", 1, 0, new object[] { }},
                new object[] {"1-alpha", "1.0-alpha", 1, 0, new object[] { "alpha" }},
                new object[] {"1.0", "1.0", 1, 0, new object[] { }},
                new object[] {"1.0-alpha", "1.0-alpha", 1, 0, new object[] {"alpha"}},
                new object[] {"1.0-alpha.1", "1.0-alpha.1", 1, 0, new object[] {"alpha", 1}},
                new object[] {"1.0-alpha.beta", "1.0-alpha.beta", 1, 0, new object[] {"alpha", "beta"}},
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData("1.2.3")]
        [InlineData("1-a-b")]
        [InlineData("1x")]
        [InlineData("x")]
        [InlineData("x1")]
        public void ShouldNotParse(string candidate)
        {
            new SemVerApiVersionParser().TryParse(candidate, out var version).ShouldBeFalse();
            version.ShouldBeNull();
        }
    }
}