namespace HandymanExtensionsTests;

public class ConcurrentDictionaryTests
{
    [Fact]
    public void AddOrUpdate()
    {
        var dictionary = new ConcurrentDictionary<int, string>();

        dictionary.AddOrUpdate(1, "one");

        dictionary.GetOrThrow(1).ShouldBe("one");

        dictionary.AddOrUpdate(1, "ONE");

        dictionary.GetOrThrow(1).ShouldBe("ONE");
    }

    [Fact]
    public void GetOrThrow()
    {
        var dictionary = new ConcurrentDictionary<int, string>();

        Should.Throw<KeyNotFoundException>(() => dictionary.GetOrThrow(1));

        dictionary.AddOrUpdate(1, "one");

        dictionary.GetOrThrow(1).ShouldBe("one");
    }
}