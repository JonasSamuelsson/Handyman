using Handyman.AspNetCore.ETags.Internals;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests.ETags;

public class ETagConverterTests
{
    [Theory]
    [InlineData(new byte[] { 1, 12, 123 }, "W/\"010c7b\"")]
    [InlineData(new byte[] { 0, 1, 12, 123 }, "W/\"010c7b\"")]
    public void ShouldConvertFromByteArray(byte[] bytes, string eTag)
    {
        new ETagConverter().FromByteArray(bytes).ShouldBe(eTag);
    }
}