using System;
using Shouldly;

namespace Handyman.Tests
{
    public class DoubleExtensionsTests
    {
        public void ShouldConvertDoubleToMilliseconds()
        {
            1.5.Milliseconds().ShouldBe(TimeSpan.FromMilliseconds(1.5));
        }

        public void ShouldConvertDoubleToSeconds()
        {
            1.5.Seconds().ShouldBe(TimeSpan.FromSeconds(1.5));
        }

        public void ShouldConvertDoubleToMinutes()
        {
            1.5.Minutes().ShouldBe(TimeSpan.FromMinutes(1.5));
        }

        public void ShouldConvertDoubleToHours()
        {
            1.5.Hours().ShouldBe(TimeSpan.FromHours(1.5));
        }

        public void ShouldConvertDoubleToDays()
        {
            1.5.Days().ShouldBe(TimeSpan.FromDays(1.5));
        }

        public void ShouldConvertDoubleToWeeks()
        {
            2.0.Weeks().ShouldBe(TimeSpan.FromDays(14));
        }

        public void ShouldCheckIfNumberIsBetween()
        {
            0.0.IsBetween(1, 3).ShouldBe(false);
            1.0.IsBetween(1, 3).ShouldBe(false);
            2.0.IsBetween(1, 3).ShouldBe(true);
            3.0.IsBetween(1, 3).ShouldBe(false);
            4.0.IsBetween(1, 3).ShouldBe(false);
        }

        public void ShouldCheckIfNumberIsInRange()
        {
            0.0.IsInRange(1, 3).ShouldBe(false);
            1.0.IsInRange(1, 3).ShouldBe(true);
            2.0.IsInRange(1, 3).ShouldBe(true);
            3.0.IsInRange(1, 3).ShouldBe(true);
            4.0.IsInRange(1, 3).ShouldBe(false);
        }
    }
}