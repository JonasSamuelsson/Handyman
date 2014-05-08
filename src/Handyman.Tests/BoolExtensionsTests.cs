using Shouldly;

namespace Handyman.Tests
{
    public class BoolExtensionsTests
    {
        public void ShouldCheckIfBoolIsFalse()
        {
            false.IsFalse().ShouldBe(true);
            true.IsFalse().ShouldBe(false);
        }

        public void ShouldCheckIfBoolIsTrue()
        {
            false.IsTrue().ShouldBe(false);
            true.IsTrue().ShouldBe(true);
        }
    }
}