using Handyman.AspNetCore.ApiVersioning;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning
{
    public class ExactMatchValidatorTests
    {
        [Fact]
        public void MissingOptionalVersionShouldBeValid()
        {
            var version = "";
            var optional = true;
            var validVersions = new[] { "1" };

            new ExactMatchValidator().Validate(version, optional, validVersions, out var matchedVersion, out _)
                .ShouldBeTrue();

            matchedVersion.ShouldBeNull();
        }

        [Fact]
        public void MissingRequiredVersionShouldBeInvalid()
        {
            var version = "";
            var optional = false;
            var validVersions = new[] { "1" };

            new ExactMatchValidator().Validate(version, optional, validVersions, out _, out _)
                .ShouldBeFalse();
        }

        [Fact]
        public void VersionsNotMatchingShouldBeInvalid()
        {
            var version = "0";
            var optional = true;
            var validVersions = new[] { "1" };

            new ExactMatchValidator().Validate(version, optional, validVersions, out _, out _)
                .ShouldBeFalse();
        }

        [Theory, MemberData(nameof(GetMatchingVersionsShouldBeValidParams))]
        public void MatchingVersionsShouldBeValid(string version)
        {
            var optional = true;
            var validVersions = new[] { "1", "a", "this-is-a-long-version" };

            new ExactMatchValidator().Validate(version, optional, validVersions, out var matchedVersion, out _)
                .ShouldBeTrue();

            matchedVersion.ShouldBe(version);
        }

        public static IEnumerable<object[]> GetMatchingVersionsShouldBeValidParams()
        {
            return new[]
            {
                new object[] {"1"},
                new object[] {"a"},
                new object[] {"this-is-a-long-version"}
            };
        }
    }
}
