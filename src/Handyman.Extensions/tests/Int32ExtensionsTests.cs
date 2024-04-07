namespace Handyman.Extensions.Tests
{
    public class Int32ExtensionsTests
    {
        [Fact]
        public void ShouldConvertIntToTicks()
        {
            1.Ticks().ShouldBe(TimeSpan.FromTicks(1));
        }

        [Fact]
        public void ShouldConvertIntToMilliseconds()
        {
            1.Milliseconds().ShouldBe(TimeSpan.FromMilliseconds(1));
        }

        [Fact]
        public void ShouldConvertIntToSeconds()
        {
            1.Seconds().ShouldBe(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void ShouldConvertIntToMinutes()
        {
            1.Minutes().ShouldBe(TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void ShouldConvertIntToHours()
        {
            1.Hours().ShouldBe(TimeSpan.FromHours(1));
        }

        [Fact]
        public void ShouldConvertIntToDays()
        {
            1.Days().ShouldBe(TimeSpan.FromDays(1));
        }

        [Fact]
        public void ShouldConvertIntToWeeks()
        {
            2.Weeks().ShouldBe(TimeSpan.FromDays(14));
        }
    }
}