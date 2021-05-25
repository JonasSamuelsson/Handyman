using Handyman.AspNetCore.ETags;
using Handyman.AspNetCore.ETags.Internals;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests.ETags
{
    public class ETagConverterTests
    {
        [Theory]
        [InlineData(new byte[] { 0 }, "W/\"00\"")]
        [InlineData(new byte[] { 0, 1, 12, 123 }, "W/\"00010c7b\"")]
        public void ShouldConvertFromByteArray(byte[] bytes, string eTag)
        {
            new ETagConverter().FromByteArray(bytes).ShouldBe(eTag);
        }
    }
}