namespace HandymanExtensionsTests;

public class RandomTests
{
    [Fact]
    public void ShouldReturnByteArrayWithSpecifiedLength()
    {
        var bytes = new Random().NextBytes(12);

        bytes.Length.ShouldBe(12);
        bytes.Any(x => x != 0);
    }
}