namespace Handyman.Extensions.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void ShouldJoinStrings()
    {
        new[] { "1", "2", "3" }.Join().ShouldBe("123");
        new[] { "join", "multiple", "strings" }.Join(" ").ShouldBe("join multiple strings");
    }

    [Fact]
    public void ShouldCheckIfStringContainsValue()
    {
        "Hello World".Contains("lo wo", StringComparison.InvariantCulture).ShouldBe(false);
        "Hello World".Contains("lo wo", StringComparison.InvariantCultureIgnoreCase).ShouldBe(true);
    }

    [Fact]
    public void ShouldCheckIfStringIsNull()
    {
        ((string)null).IsNull().ShouldBe(true);
        "".IsNull().ShouldBe(false);
    }

    [Fact]
    public void ShouldCheckIfStringIsEmpty()
    {
        ((string)null).IsEmpty().ShouldBe(false);
        "".IsEmpty().ShouldBe(true);
        " ".IsEmpty().ShouldBe(false);
        "x".IsEmpty().ShouldBe(false);
    }

    [Fact]
    public void ShouldCheckIfStringIsWhiteSpace()
    {
        ((string)null).IsWhiteSpace().ShouldBe(false);
        "".IsWhiteSpace().ShouldBe(true);
        " ".IsWhiteSpace().ShouldBe(true);
        "x".IsWhiteSpace().ShouldBe(false);
    }

    [Fact]
    public void ShouldCheckIfStringIsNullOrEmpty()
    {
        ((string)null).IsNullOrEmpty().ShouldBe(true);
        "".IsNullOrEmpty().ShouldBe(true);
        " ".IsNullOrEmpty().ShouldBe(false);
        "x".IsNullOrEmpty().ShouldBe(false);
    }

    [Fact]
    public void ShouldCheckIfStringIsNullOrWhiteSpace()
    {
        ((string)null).IsNullOrWhiteSpace().ShouldBe(true);
        "".IsNullOrWhiteSpace().ShouldBe(true);
        " ".IsNullOrWhiteSpace().ShouldBe(true);
        "x".IsNullOrWhiteSpace().ShouldBe(false);
    }

    [Fact]
    public void ShouldCheckIfStringIsNotNull()
    {
        ((string)null).IsNotNull().ShouldBe(false);
        "".IsNotNull().ShouldBe(true);
    }

    [Fact]
    public void ShouldCheckIfStringIsNotEmpty()
    {
        ((string)null).IsNotEmpty().ShouldBe(true);
        "".IsNotEmpty().ShouldBe(false);
        " ".IsNotEmpty().ShouldBe(true);
        "x".IsNotEmpty().ShouldBe(true);
    }

    [Fact]
    public void ShouldCheckIfStringIsNotWhiteSpace()
    {
        ((string)null).IsNotWhiteSpace().ShouldBe(true);
        "".IsNotWhiteSpace().ShouldBe(false);
        " ".IsNotWhiteSpace().ShouldBe(false);
        "x".IsNotWhiteSpace().ShouldBe(true);
    }

    [Fact]
    public void ShouldCheckIfStringIsNotNullOrEmpty()
    {
        ((string)null).IsNotNullOrEmpty().ShouldBe(false);
        "".IsNotNullOrEmpty().ShouldBe(false);
        " ".IsNotNullOrEmpty().ShouldBe(true);
        "x".IsNotNullOrEmpty().ShouldBe(true);
    }

    [Fact]
    public void ShouldCheckIfStringIsNotNullOrWhiteSpace()
    {
        ((string)null).IsNotNullOrWhiteSpace().ShouldBe(false);
        "".IsNotNullOrWhiteSpace().ShouldBe(false);
        " ".IsNotNullOrWhiteSpace().ShouldBe(false);
        "x".IsNotNullOrWhiteSpace().ShouldBe(true);
    }

    [Fact]
    public void ShouldGetSubstring()
    {
        "".SubstringSafe(1).ShouldBe(string.Empty);
        "".SubstringSafe(1, 1).ShouldBe(string.Empty);
        "Hello".SubstringSafe(1).ShouldBe("ello");
        "Hello".SubstringSafe(1, 1).ShouldBe("e");
    }

    [Fact]
    public void ShouldReverseTheString()
    {
        "Hello".Reverse().ShouldBe("olleH");
    }

    [Fact]
    public void ShouldConvertStringToEnumOrThrow()
    {
        Should.Throw<ArgumentException>(() => "yes".ToEnum<IgnoreCase>());
        "yes".ToEnum<IgnoreCase>(IgnoreCase.Yes).ShouldBe(IgnoreCase.Yes);
    }

    [Fact]
    public void ShouldConvertStringToEnumOrNull()
    {
        "yes".ToEnumOrNull<IgnoreCase>().ShouldBe(null);
        "yes".ToEnumOrNull<IgnoreCase>(IgnoreCase.Yes).ShouldBe(IgnoreCase.Yes);
    }

    [Fact]
    public void ShouldConvertStringToEnumOrDefault()
    {
        "yes".ToEnumOrDefault(IgnoreCase.No).ShouldBe(IgnoreCase.No);
        "yes".ToEnumOrDefault(() => IgnoreCase.No).ShouldBe(IgnoreCase.No);
    }

    [Fact]
    public void ShouldTryConvertStringToEnum()
    {
        int @int;
        Should.Throw<ArgumentException>(() => "one".TryToEnum(out @int));
        Should.Throw<ArgumentException>(() => "one".TryToEnum(IgnoreCase.Yes, out @int));

        // ReSharper disable once RedundantAssignment
        var number = Number.One;

        "one".TryToEnum(out number).ShouldBe(false);
        number.ShouldBe(Number.Zero);

        "one".TryToEnum(IgnoreCase.Yes, out number).ShouldBe(true);
        number.ShouldBe(Number.One);

        // ReSharper disable once RedundantAssignment
        number = Number.Zero;
        "1".TryToEnum(out number).ShouldBe(true);
        number.ShouldBe(Number.One);
    }

    private enum Number
    {
        Zero = 0,
        One = 1
    }

    [Fact]
    public void ShouldSplitString()
    {
        Should.Throw<ArgumentException>(() => "".Split(0).Enumerate())
            .Message.ShouldStartWith("Length must be greater than 1.");

        "".Split(1).ShouldBe(new string[] { });

        "123".Split(1).ShouldBe(new[] { "1", "2", "3" });
        "123".Split(2).ShouldBe(new[] { "12", "3" });
        "123".Split(3).ShouldBe(new[] { "123" });
        "123".Split(4).ShouldBe(new[] { "123" });
    }
}