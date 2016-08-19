using Shouldly;

namespace Handyman.Tests.Core
{
    public class ComparableExtensionsTests
    {
        public void ShouldCheckIfValueIsBetween()
        {
            1.IsBetween(2, 4).ShouldBe(false);
            2.IsBetween(2, 4).ShouldBe(false);
            3.IsBetween(2, 4).ShouldBe(true);
            4.IsBetween(2, 4).ShouldBe(false);
            5.IsBetween(2, 4).ShouldBe(false);

            1.1.IsBetween(1.2, 1.4).ShouldBe(false);
            1.2.IsBetween(1.2, 1.4).ShouldBe(false);
            1.3.IsBetween(1.2, 1.4).ShouldBe(true);
            1.4.IsBetween(1.2, 1.4).ShouldBe(false);
            1.5.IsBetween(1.2, 1.4).ShouldBe(false);
        }

        public void ShouldCheckIfValueIsInRange()
        {
            1.IsInRange(2, 4).ShouldBe(false);
            2.IsInRange(2, 4).ShouldBe(true);
            3.IsInRange(2, 4).ShouldBe(true);
            4.IsInRange(2, 4).ShouldBe(true);
            5.IsInRange(2, 4).ShouldBe(false);

            1.1.IsInRange(1.2, 1.4).ShouldBe(false);
            1.2.IsInRange(1.2, 1.4).ShouldBe(true);
            1.3.IsInRange(1.2, 1.4).ShouldBe(true);
            1.4.IsInRange(1.2, 1.4).ShouldBe(true);
            1.5.IsInRange(1.2, 1.4).ShouldBe(false);
        }

        public void ShouldClamp()
        {
            1.Clamp(2, 4).ShouldBe(2);
            2.Clamp(2, 4).ShouldBe(2);
            3.Clamp(2, 4).ShouldBe(3);
            4.Clamp(2, 4).ShouldBe(4);
            5.Clamp(2, 4).ShouldBe(4);

            1.Clamp(4, 2).ShouldBe(2);
            2.Clamp(4, 2).ShouldBe(2);
            3.Clamp(4, 2).ShouldBe(3);
            4.Clamp(4, 2).ShouldBe(4);
            5.Clamp(4, 2).ShouldBe(4);
        }
    }
}