using Shouldly;

namespace Handyman.Tests
{
    public class ComparableTests
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

            "a".IsBetween("b", "d").ShouldBe(false);
            "b".IsBetween("b", "d").ShouldBe(false);
            "c".IsBetween("b", "d").ShouldBe(true);
            "d".IsBetween("b", "d").ShouldBe(false);
            "e".IsBetween("b", "d").ShouldBe(false);
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

            "a".IsInRange("b", "d").ShouldBe(false);
            "b".IsInRange("b", "d").ShouldBe(true);
            "c".IsInRange("b", "d").ShouldBe(true);
            "d".IsInRange("b", "d").ShouldBe(true);
            "e".IsInRange("b", "d").ShouldBe(false);
        }
    }
}