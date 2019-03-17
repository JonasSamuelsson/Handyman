using Handyman.AspNetCore.ApiVersioning;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.AspNetCore.Tests
{
    public class SemanticVersionComparerTests
    {
        [Theory, MemberData(nameof(GetShouldCompareSemanticVersionsParams))]
        internal void ShouldCompareSemanticVersions(SemanticVersion x, SemanticVersion y, int result)
        {
            SemanticVersionComparer.Default.Compare(x, y).ShouldBe(result);
        }

        public static IEnumerable<object[]> GetShouldCompareSemanticVersionsParams()
        {
            var versions = new[]
            {
                "1.0-alpha",
                "1.0-alpha.2",
                "1.0-alpha.11",
                "1.0-alpha.beta",
                "1.0-beta",
                "1.0",
                "2.0-alpha",
                "2.0-alpha.2",
                "2.0-alpha.11",
                "2.0-alpha.beta",
                "2.0-beta",
                "2.0"
            };

            for (var x = 0; x < versions.Length; x++)
            {
                for (var y = 0; y < versions.Length; y++)
                {
                    SemanticVersionParser.TryParse(versions[x], out var semanticVersionX);
                    SemanticVersionParser.TryParse(versions[y], out var semanticVersionY);
                    var expectedResult = x.CompareTo(y);

                    yield return new object[] { semanticVersionX, semanticVersionY, expectedResult };
                }
            }
        }
    }
}