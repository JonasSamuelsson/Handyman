using Handyman.AspNetCore.ApiVersioning.SemVer;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning.SemVer
{
    public class SemVerApiVersionComparerTests
    {
        private static readonly SemVerApiVersionParser Parser = new SemVerApiVersionParser();

        [Theory, MemberData(nameof(GetCompareParams))]
        internal void Compare(SemVerApiVersion x, SemVerApiVersion y, int result)
        {
            x.CompareTo(y).ShouldBe(result);
        }

        public static IEnumerable<object[]> GetCompareParams()
        {
            var versions = new[]
            {
                "1.0-alpha",
                "1.0-beta",
                "1.0-beta.1",
                "1.0-beta.1.1",
                "1.0-beta.1.2",
                "1.0-beta.2",
                "1.0",
                "1.1-alpha",
                "1.1-beta",
                "1.1-beta.1",
                "1.1-beta.1.1",
                "1.1-beta.1.2",
                "1.1-beta.2",
                "1.1",
                "2.0-alpha",
                "2.0-beta",
                "2.0-beta.1",
                "2.0-beta.1.1",
                "2.0-beta.1.2",
                "2.0-beta.2",
                "2.0",
            };

            for (var i = 0; i < versions.Length; i++)
            {
                for (var j = 0; j < versions.Length; j++)
                {
                    var x = Parser.Parse(versions[i]);
                    var y = Parser.Parse(versions[j]);
                    var result = i.CompareTo(j);
                    yield return new object[] { x, y, result };
                }
            }
        }
    }
}