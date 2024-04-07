namespace Handyman.Extensions.Tests;

public class DoubleExtensionsTests
{
    [Fact]
    public void ShouldConvertDoubleToMilliseconds()
    {
        1.5.Milliseconds().ShouldBe(TimeSpan.FromMilliseconds(1.5));
    }

    [Fact]
    public void ShouldConvertDoubleToSeconds()
    {
        1.5.Seconds().ShouldBe(TimeSpan.FromSeconds(1.5));
    }

    [Fact]
    public void ShouldConvertDoubleToMinutes()
    {
        1.5.Minutes().ShouldBe(TimeSpan.FromMinutes(1.5));
    }

    [Fact]
    public void ShouldConvertDoubleToHours()
    {
        1.5.Hours().ShouldBe(TimeSpan.FromHours(1.5));
    }

    [Fact]
    public void ShouldConvertDoubleToDays()
    {
        1.5.Days().ShouldBe(TimeSpan.FromDays(1.5));
    }

    [Fact]
    public void ShouldConvertDoubleToWeeks()
    {
        2.0.Weeks().ShouldBe(TimeSpan.FromDays(14));
    }
}