using Handyman.AspNetCore.ETags.Internals;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests.ETags;

public class ETagComparerTests
{
    [Theory]
    [InlineData("*", "123", true)]
    [InlineData("123", "*", true)]
    [InlineData("123", "123", true)]
    [InlineData("123", "321", false)]
    public void ShouldCompareETags(string eTag1, string eTag2, bool result)
    {
        new ETagComparer().Equals(eTag1, eTag2).ShouldBe(result);
    }
}