using Shouldly;
using System;
using Xunit;

namespace Handyman.AspNetCore.Tests
{
    public class ETagConverterTests
    {
        [Theory]
        [InlineData("\"00\"")]
        [InlineData("W/\"\"")]
        [InlineData("W/\"0\"")]
        [InlineData("W/\"xx\"")]
        public void ToByteArrayShouldThrowOnMalformedEtag(string eTag)
        {
            Assert.Throws<FormatException>(() => new ETagConverter().ToByteArray(eTag));
        }

        [Theory]
        [InlineData("W/\"00\"", new byte[] { 0 })]
        [InlineData("W/\"010c7b\"", new byte[] { 1, 12, 123 })]
        public void ShouldConvertToByteArray(string eTag, byte[] bytes)
        {
            new ETagConverter().ToByteArray(eTag).ShouldBe(bytes);
        }

        [Theory]
        [InlineData(new byte[] { 0 }, "W/\"00\"")]
        [InlineData(new byte[] { 1, 12, 123 }, "W/\"010c7b\"")]
        public void ShouldConvertFromByteArray(byte[] bytes, string eTag)
        {
            new ETagConverter().FromByteArray(bytes).ShouldBe(eTag);
        }
    }
}