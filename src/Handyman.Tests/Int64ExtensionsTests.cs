using Shouldly;

namespace Handyman.Tests
{
    public class Int64ExtensionsTests
    {
        public void ShouldCheckIfNumberIsBetween()
        {
            0L.IsBetween(1, 3).ShouldBe(false);
            1L.IsBetween(1, 3).ShouldBe(false);
            2L.IsBetween(1, 3).ShouldBe(true);
            3L.IsBetween(1, 3).ShouldBe(false);
            4L.IsBetween(1, 3).ShouldBe(false);
        }

        public void ShouldCheckIfNumberIsInRange()
        {
            0L.IsInRange(1, 3).ShouldBe(false);
            1L.IsInRange(1, 3).ShouldBe(true);
            2L.IsInRange(1, 3).ShouldBe(true);
            3L.IsInRange(1, 3).ShouldBe(true);
            4L.IsInRange(1, 3).ShouldBe(false);
        } 
    }
}