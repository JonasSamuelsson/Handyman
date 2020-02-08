using System;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.AspNetCore.Tests
{
    public class ETagComparerTests
    {
        [Theory, MemberData(nameof(GetShouldCompareETagsParams))]
        public void ShouldCompareETags(string eTag1, string eTag2, bool result)
        {
            ETagComparer.Instance.Equals(eTag1, eTag2).ShouldBe(result);
        }

        public static IEnumerable<object[]> GetShouldCompareETagsParams()
        {
            var eTags = new[] { "*", "W/\"01\"", "W/\"02\"", string.Empty, null };

            for (var i = 0; i < eTags.Length; i++)
            {
                for (var j = 0; j < eTags.Length; j++)
                {
                    var eTag1 = eTags[i];
                    var eTag2 = eTags[j];
                    var result = i == 0 || j == 0 || (i == j && (i == 1 || i == 2));
                    yield return new object[] { eTag1, eTag2, result };
                }
            }
        }

        [Theory, MemberData(nameof(GetShouldCompareETagToSqlServerRowVersionParams))]
        public void ShouldCompareETagToSqlServerRowVersion(string eTag, byte[] rowVersion, bool result)
        {
            ETagComparer.Instance.EqualsSqlServerRowVersion(eTag, rowVersion).ShouldBe(result);
        }

        public static IEnumerable<object[]> GetShouldCompareETagToSqlServerRowVersionParams()
        {
            throw new NotImplementedException();

            //var rowVersion = ETagConverter.ToSqlServerRowVersion("W/\"01\"");

            //yield return new object[] { "*", rowVersion, true };
            //yield return new object[] { "W/\"01\"", rowVersion, true };
            //yield return new object[] { "W/\"02\"", rowVersion, false };
        }
    }
}