using System.Collections.Generic;
using System.Linq;
using Handyman.AspNetCore.ApiVersioning;
using Microsoft.Extensions.Primitives;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning
{
    public class SemanticVersionParserTests
    {
        [Theory, MemberData(nameof(GetShouldTryParseParams))]
        public void ShouldTryParse(string s, long major, long minor, string preRelease)
        {
            SemanticVersionParser.TryParse(s, out var semanticVersion)
                .ShouldBeTrue();

            semanticVersion.Major.ShouldBe(major);
            semanticVersion.Minor.ShouldBe(minor);
            semanticVersion.PreRelease.ShouldBe(preRelease);
        }

        public static IEnumerable<object[]> GetShouldTryParseParams()
        {
            return new[]
            {
                new object[] {"1", 1, 0, ""},
                new object[] {"1.2", 1, 2, ""},
                new object[] {"1.2-3", 1, 2, "3"},
                new object[] {"1.2-3.a.4", 1, 2, "3.a.4"},
                new object[] {"1.2-a", 1, 2, "a"},
                new object[] {"1.2-a.3.b", 1, 2, "a.3.b"}
            };
        }

        [Theory, MemberData(nameof(GetShouldParseParams))]
        public void ShouldParse(StringValues versions, string[] strings)
        {
            var result = SemanticVersionParser.Parse(versions);

            result.DeclaredVersions.Select(x => x.SemanticVersion.ToString()).ShouldBe(new[] { "1.0", "2.0-alpha", "2.0", "3.0-beta" });
            result.DeclaredVersions.Select(x => x.String).ShouldBe(strings);
            result.ValidationError.ShouldBe("Invalid api version, supported semantic versions: 1.0, 2.0, 3.0-beta.");
        }

        public static IEnumerable<object[]> GetShouldParseParams()
        {
            return new[]
            {
                new object[] {(StringValues)new[] {"1", "2-alpha", "2", "3-beta"}, new[] {"1", "2-alpha", "2", "3-beta"}},
                new object[] {(StringValues)new[] {"1", "2-alpha", "2", "3-beta"}, new[] {"1", "2-alpha", "2", "3-beta"}}, // yes, its the same strings again
                new object[] {(StringValues)new[] {"3.0-beta", "2.0", "2.0-alpha", "1.0"}, new[] { "1.0", "2.0-alpha", "2.0", "3.0-beta" } }
            };
        }
    }
}