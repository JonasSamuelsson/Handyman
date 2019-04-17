using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Handyman.AspNetCore.Tests
{
    public class ETagConverterTests
    {
        [Theory, MemberData(nameof(GetConvertToSqlServerRowVersionShouldThrowOnMalformedEtagParams))]
        public void ConvertToSqlServerRowVersionShouldThrowOnMalformedEtag(string s)
        {
            Assert.Throws<FormatException>(() => ETagConverter.ToSqlServerRowVersion(null));
            Assert.Throws<FormatException>(() => ETagConverter.ToSqlServerRowVersion(""));
            Assert.Throws<FormatException>(() => ETagConverter.ToSqlServerRowVersion("1"));
            Assert.Throws<FormatException>(() => ETagConverter.ToSqlServerRowVersion("01\""));
            Assert.Throws<FormatException>(() => ETagConverter.ToSqlServerRowVersion("W/\"\""));
            Assert.Throws<FormatException>(() => ETagConverter.ToSqlServerRowVersion("W/\"1\""));
            Assert.Throws<FormatException>(() => ETagConverter.ToSqlServerRowVersion("W/\"01020304050607080\""));
        }

        public static IEnumerable<object[]> GetConvertToSqlServerRowVersionShouldThrowOnMalformedEtagParams()
        {
            return new[]
            {
                new object[] {null},
                new object[] {""},
                new object[] {"1"},
                new object[] {"01\""},
                new object[] {"W/\"\""},
                new object[] {"W/\"1\""},
                new object[] {"W/\"01020304050607080\""}
            };
        }

        [Theory, MemberData(nameof(GetShouldConvertToSqlServerRowVersionParams))]
        public void ShouldConvertToSqlServerRowVersion(string etag, byte[] rowVersion)
        {
            ETagConverter.ToSqlServerRowVersion(etag).ShouldBe(rowVersion);
        }

        public static IEnumerable<object[]> GetShouldConvertToSqlServerRowVersionParams()
        {
            return new[]
            {
                new object[] {"W/\"010c7b\"", new byte[] {0, 0, 0, 0, 0, 1, 12, 123}},
                new object[] {"W/\"010c7b00\"", new byte[] {0, 0, 0, 0, 1, 12, 123, 0}}
            };
        }

        [Theory, MemberData(nameof(GetShouldConvertFromSqlServerRowVersionParams))]
        public void ShouldConvertFromSqlServerRowVersion(byte[] rowVersion, string etag)
        {
            ETagConverter.FromSqlServerRowVersion(rowVersion).ShouldBe(etag);
        }

        public static IEnumerable<object[]> GetShouldConvertFromSqlServerRowVersionParams()
        {
            return new[]
            {
                new object[]{new byte[] { 0, 0, 0, 0, 0, 1, 12, 123 }, "W/\"010c7b\""},
                new object[]{new byte[] { 0, 0, 0, 0, 1, 12, 123, 0 }, "W/\"010c7b00\"" }
            };
        }
    }
}