using System.Collections.Generic;
using System.Linq;
using Handyman.AspNetCore.ApiVersioning;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning
{
    public class SemanticVersionValidatorTests
    {
        [Theory, MemberData(nameof(GetUnmatchingVersionShouldBeInvalidParams))]
        public void UnmatchingVersionShouldBeInvalid(string version, bool optional, string[] validVersions)
        {
            new SemanticVersionValidator().Validate(version, optional, validVersions, out var matchedVersion, out var error)
                .ShouldBeFalse();

            matchedVersion.ShouldBeNull();
        }

        public static IEnumerable<object[]> GetUnmatchingVersionShouldBeInvalidParams()
        {
            var testCases = new[]
            {
                new object[] {"1.0-alpha.1", new []{"1.0-alpha"}},
                new object[] {"1.0-beta", new []{"1.0-alpha"}},
                new object[] {"1.0", new []{ "1.0-alpha" } },
                new object[] {"1.1", new []{"1.0"}},
                new object[] {"2.0", new []{"1.0"}},
                new object[] {"2.0", new []{"1.1"}}
            };

            foreach (var optional in new[] { false, true })
            {
                foreach (var values in testCases)
                {
                    var @params = values.ToList();
                    @params.Insert(1, optional);
                    yield return @params.ToArray();
                }
            }
        }

        [Fact]
        public void MissingOptionalVersionShouldBeValid()
        {
            var version = "";
            var optional = true;
            var validVersions = new[] { "1" };

            new SemanticVersionValidator().Validate(version, optional, validVersions, out var matchedVersion, out var error)
                .ShouldBeTrue();

            matchedVersion.ShouldBeNull();
        }

        [Theory, MemberData(nameof(GetMatchingVersionShouldBeValidParams))]
        public void MatchingVersionShouldBeValid(string version, bool optional, string[] validVersions, string expectedMatchedVersion)
        {
            new SemanticVersionValidator().Validate(version, optional, validVersions, out var matchedVersion, out var error)
                .ShouldBeTrue();

            matchedVersion.ShouldBe(expectedMatchedVersion);
        }

        public static IEnumerable<object[]> GetMatchingVersionShouldBeValidParams()
        {
            var testCases = new[]
            {
                new object[] {"1", new []{"1"}, "1"},
                new object[] {"1", new []{"1.0"}, "1.0"},
                new object[] {"1.0", new []{"1"}, "1"},
                new object[] {"1-alpha", new []{"1-alpha"}, "1-alpha"},
                new object[] {"1-alpha", new []{"1.0-alpha"}, "1.0-alpha"},
                new object[] {"1.0-alpha", new []{"1-alpha"}, "1-alpha"},
                new object[] {"1.0-alpha", new []{"1.0-alpha"}, "1.0-alpha"},
            };

            foreach (var optional in new[] { false, true })
            {
                foreach (var values in testCases)
                {
                    var @params = values.ToList();
                    @params.Insert(1, optional);
                    yield return @params.ToArray();
                }
            }
        }

        [Fact]
        public void MissingRequiredVersionShouldNotBeValid()
        {
            var version = "";
            var optional = false;
            var validVersions = new[] { "1" };

            new SemanticVersionValidator().Validate(version, optional, validVersions, out var matchedVersion, out var error)
                .ShouldBeFalse();

            matchedVersion.ShouldBeNull();
        }
    }
}