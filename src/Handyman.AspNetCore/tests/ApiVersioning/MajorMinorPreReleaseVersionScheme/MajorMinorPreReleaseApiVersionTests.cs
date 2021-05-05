using Handyman.AspNetCore.ApiVersioning.Internals.MajorMinorPreReleaseVersionScheme;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning.MajorMinorPreReleaseVersionScheme
{
    public class MajorMinorPreReleaseApiVersionTests
    {
        [Theory]
        [MemberData(nameof(GetParams))]
        public void ShouldCompareVersions(string versionX, string versionY, bool result)
        {
            var parser = new MajorMinorPreReleaseApiVersionParser();
            parser.TryParse(versionX, out var apiVersionX).ShouldBeTrue();
            parser.TryParse(versionY, out var apiVersionY).ShouldBeTrue();
            apiVersionX.IsMatch(apiVersionY).ShouldBe(result);
        }

        public static IEnumerable<object[]> GetParams()
        {
            var majors = new[] { "1", "2" };
            var minors = new[] { "", ".0", ".1" };
            var preReleases = new[] { "", "-alpha", "-ALPHA", "-beta" };

            var versions = (from major in majors from minor in minors from preRelease in preReleases select $"{major}{minor}{preRelease}").ToList();

            foreach (var x in versions)
            {
                foreach (var y in versions)
                {
                    yield return new object[] { x, y, x.Equals(y, StringComparison.OrdinalIgnoreCase) };
                }
            }
        }
    }
}