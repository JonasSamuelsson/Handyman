using Handyman.AspNetCore.ApiVersioning.SemVer;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning.SemVer
{
    public class SemVerApiVersionComparerTests
    {
        private static readonly SemVerApiVersionParser Parser = new SemVerApiVersionParser();

        [Theory, MemberData(nameof(GetCompareToParams))]
        internal void CompareTo(SemVerApiVersion x, SemVerApiVersion y, int result)
        {
            x.CompareTo(y).ShouldBe(result);
        }

        public static IEnumerable<object[]> GetCompareToParams()
        {
            var versions = new[]
            {
                "1.0-alpha",
                "1.0-beta",
                "1.0-beta.1",
                "1.0-beta.2",
                "1.0-beta.10",
                "1.0",
                "1.1-alpha",
                "1.1",
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

        [Theory, MemberData(nameof(GetIsMatchParams))]
        internal void IsMatch(SemVerApiVersion version, SemVerApiVersion candidate, bool result)
        {
            version.IsMatch(candidate).ShouldBe(result);
        }

        public static IEnumerable<object[]> GetIsMatchParams()
        {
            var items = new[]
            {
                new object[] {"1.0-alpha", "1.0-alpha", true},
                new object[] {"1.0-alpha", "1.0-beta", false},

                new object[] {"1.0-beta", "1.0-alpha", false},
                new object[] {"1.0-beta", "1.0-beta", true},
                new object[] {"1.0-beta", "1.0", false},

                new object[] {"1.0", "1.0-alpha", false},
                new object[] {"1.0", "1.0", true},
                new object[] {"1.0", "1.1", false},
                new object[] {"1.0", "2.0", false},

                new object[] {"1.1", "1.0", true},
                new object[] {"1.1", "1.1", true},
                new object[] {"1.1", "2.0", false},

                new object[] {"2.0", "1.1", false},
                new object[] {"2.0", "2.0", true},
            };

            var parser = new SemVerApiVersionParser();

            foreach (var item in items)
            {
                var version = parser.Parse((string)item[0]);
                var candidate = parser.Parse((string)item[1]);
                yield return new[] { version, candidate, item[2] };
            }
        }
    }
}