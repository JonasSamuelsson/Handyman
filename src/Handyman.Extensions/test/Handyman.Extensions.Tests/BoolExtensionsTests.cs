using Shouldly;
using Xunit;

namespace Handyman.Extensions.Tests
{
    public class BoolExtensionsTests
    {
        [Fact]
        public void ShouldCheckIfBoolIsFalse()
        {
            false.IsFalse().ShouldBe(true);
            true.IsFalse().ShouldBe(false);
        }

        [Fact]
        public void ShouldCheckIfBoolIsTrue()
        {
            false.IsTrue().ShouldBe(false);
            true.IsTrue().ShouldBe(true);
        }

        [Fact]
        public void ShouldInvertValue()
        {
            false.Not().ShouldBe(true);
            true.Not().ShouldBe(false);
        }
    }
}