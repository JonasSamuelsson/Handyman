using Handyman.AspNetCore.ETags;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests
{
    public class ETagComparerTests
    {
        [Theory]
        [InlineData(null, null, false)]
        [InlineData(null, "*", true)]
        [InlineData(null, "123", false)]
        [InlineData("*", null, true)]
        [InlineData("*", "123", true)]
        [InlineData("123", null, false)]
        [InlineData("123", "*", true)]
        [InlineData("123", "123", true)]
        [InlineData("123", "321", false)]
        public void ShouldCompareETags(string eTag1, string eTag2, bool result)
        {
            new ETagComparer(new ETagConverter()).Equals(eTag1, eTag2).ShouldBe(result);
        }
    }
}