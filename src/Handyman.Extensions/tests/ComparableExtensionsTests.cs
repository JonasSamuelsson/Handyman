namespace HandymanExtensionsTests;

public class ComparableExtensionsTests
{
    [Fact]
    public void ShouldCheckIfValueIsInRange()
    {
        1.IsInRange(2, 4, RangeBounds.Inclusive).ShouldBe(false);
        2.IsInRange(2, 4, RangeBounds.Inclusive).ShouldBe(true);
        3.IsInRange(2, 4, RangeBounds.Inclusive).ShouldBe(true);
        4.IsInRange(2, 4, RangeBounds.Inclusive).ShouldBe(true);
        5.IsInRange(2, 4, RangeBounds.Inclusive).ShouldBe(false);

        1.IsInRange(2, 4, RangeBounds.IncludeLower).ShouldBe(false);
        2.IsInRange(2, 4, RangeBounds.IncludeLower).ShouldBe(true);
        3.IsInRange(2, 4, RangeBounds.IncludeLower).ShouldBe(true);
        4.IsInRange(2, 4, RangeBounds.IncludeLower).ShouldBe(false);
        5.IsInRange(2, 4, RangeBounds.IncludeLower).ShouldBe(false);

        1.IsInRange(2, 4, RangeBounds.IncludeUpper).ShouldBe(false);
        2.IsInRange(2, 4, RangeBounds.IncludeUpper).ShouldBe(false);
        3.IsInRange(2, 4, RangeBounds.IncludeUpper).ShouldBe(true);
        4.IsInRange(2, 4, RangeBounds.IncludeUpper).ShouldBe(true);
        5.IsInRange(2, 4, RangeBounds.IncludeUpper).ShouldBe(false);

        1.IsInRange(2, 4, RangeBounds.Exclusive).ShouldBe(false);
        2.IsInRange(2, 4, RangeBounds.Exclusive).ShouldBe(false);
        3.IsInRange(2, 4, RangeBounds.Exclusive).ShouldBe(true);
        4.IsInRange(2, 4, RangeBounds.Exclusive).ShouldBe(false);
        5.IsInRange(2, 4, RangeBounds.Exclusive).ShouldBe(false);

        1.IsInRange(2, 4).ShouldBe(false);
        2.IsInRange(2, 4).ShouldBe(true);
        3.IsInRange(2, 4).ShouldBe(true);
        4.IsInRange(2, 4).ShouldBe(true);
        5.IsInRange(2, 4).ShouldBe(false);

        1.IsInRange(4, 2, RangeBounds.Inclusive).ShouldBe(false);
        2.IsInRange(4, 2, RangeBounds.Inclusive).ShouldBe(true);
        3.IsInRange(4, 2, RangeBounds.Inclusive).ShouldBe(true);
        4.IsInRange(4, 2, RangeBounds.Inclusive).ShouldBe(true);
        5.IsInRange(4, 2, RangeBounds.Inclusive).ShouldBe(false);

        1.IsInRange(4, 2, RangeBounds.IncludeLower).ShouldBe(false);
        2.IsInRange(4, 2, RangeBounds.IncludeLower).ShouldBe(true);
        3.IsInRange(4, 2, RangeBounds.IncludeLower).ShouldBe(true);
        4.IsInRange(4, 2, RangeBounds.IncludeLower).ShouldBe(false);
        5.IsInRange(4, 2, RangeBounds.IncludeLower).ShouldBe(false);

        1.IsInRange(4, 2, RangeBounds.IncludeUpper).ShouldBe(false);
        2.IsInRange(4, 2, RangeBounds.IncludeUpper).ShouldBe(false);
        3.IsInRange(4, 2, RangeBounds.IncludeUpper).ShouldBe(true);
        4.IsInRange(4, 2, RangeBounds.IncludeUpper).ShouldBe(true);
        5.IsInRange(4, 2, RangeBounds.IncludeUpper).ShouldBe(false);

        1.IsInRange(4, 2, RangeBounds.Exclusive).ShouldBe(false);
        2.IsInRange(4, 2, RangeBounds.Exclusive).ShouldBe(false);
        3.IsInRange(4, 2, RangeBounds.Exclusive).ShouldBe(true);
        4.IsInRange(4, 2, RangeBounds.Exclusive).ShouldBe(false);
        5.IsInRange(4, 2, RangeBounds.Exclusive).ShouldBe(false);

        1.IsInRange(4, 2).ShouldBe(false);
        2.IsInRange(4, 2).ShouldBe(true);
        3.IsInRange(4, 2).ShouldBe(true);
        4.IsInRange(4, 2).ShouldBe(true);
        5.IsInRange(4, 2).ShouldBe(false);
    }

    [Fact]
    public void IsInRangeShouldThrowIfAnyParameterIsNull()
    {
        Comparable @null = null;
        Comparable instance = new Comparable();

        Should.Throw<ArgumentNullException>(() => @null.IsInRange(instance, instance));
        Should.Throw<ArgumentNullException>(() => instance.IsInRange(@null, instance));
        Should.Throw<ArgumentNullException>(() => instance.IsInRange(instance, @null));
    }

    private class Comparable : IComparable<Comparable>
    {
        public int CompareTo(Comparable other)
        {
            return 0;
        }
    }

    [Fact]
    public void Clamp()
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

    [Fact]
    public void ClampShouldThrowIfAnyParameterIsNull()
    {
        string @null = null;

        Should.Throw<ArgumentNullException>(() => @null.Clamp("", ""));
        Should.Throw<ArgumentNullException>(() => "".Clamp(@null, ""));
        Should.Throw<ArgumentNullException>(() => "".Clamp("", @null));
    }
}