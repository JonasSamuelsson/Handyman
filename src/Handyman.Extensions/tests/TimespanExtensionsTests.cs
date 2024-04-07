namespace Handyman.Extensions.Tests;

public class TimespanExtensionsTests
{
    [Fact]
    public void ShouldGetDateTimeOffsetInThePast()
    {
        var now = Configuration.Now();
        Configuration.Now = () => now;
        10.Minutes().Ago().ShouldBe(Configuration.Now().Subtract(10.Minutes()));
    }

    [Fact]
    public void ShouldGetDateTimeOffsetInTheFuture()
    {
        var now = Configuration.Now();
        Configuration.Now = () => now;
        10.Minutes().FromNow().ShouldBe(Configuration.Now().Add(10.Minutes()));
    }
}