using System;
using Handyman.Extensions;
using Shouldly;

namespace Handyman.Tests.Extensions
{
    public class Int32ExtensionsTests
    {
        public void ShouldConvertIntToTicks()
        {
            1.Ticks().ShouldBe(TimeSpan.FromTicks(1));
        }

        public void ShouldConvertIntToMilliseconds()
        {
            1.Milliseconds().ShouldBe(TimeSpan.FromMilliseconds(1));
        }

        public void ShouldConvertIntToSeconds()
        {
            1.Seconds().ShouldBe(TimeSpan.FromSeconds(1));
        }

        public void ShouldConvertIntToMinutes()
        {
            1.Minutes().ShouldBe(TimeSpan.FromMinutes(1));
        }

        public void ShouldConvertIntToHours()
        {
            1.Hours().ShouldBe(TimeSpan.FromHours(1));
        }

        public void ShouldConvertIntToDays()
        {
            1.Days().ShouldBe(TimeSpan.FromDays(1));
        }

        public void ShouldConvertIntToWeeks()
        {
            2.Weeks().ShouldBe(TimeSpan.FromDays(14));
        }
    }
}