using System;

namespace Handyman.Extensions;

public static class ComparableExtensions
{
    public static T Clamp<T>([NotNull] this T value, [NotNull] T lower, [NotNull] T upper) where T : IComparable<T>
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (lower == null) throw new ArgumentNullException(nameof(lower));
        if (upper == null) throw new ArgumentNullException(nameof(upper));

        Order(ref lower, ref upper);

        return value.CompareTo(lower) == -1
            ? lower
            : (upper.CompareTo(value) == -1
                ? upper
                : value);
    }

    public static bool IsInRange<T>([NotNull] this T value, [NotNull] T lower, [NotNull] T upper) where T : IComparable<T>
    {
        return value.IsInRange(lower, upper, RangeBounds.Inclusive);
    }

    public static bool IsInRange<T>([NotNull] this T value, [NotNull] T lower, [NotNull] T upper, RangeBounds bounds) where T : IComparable<T>
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (lower == null) throw new ArgumentNullException(nameof(lower));
        if (upper == null) throw new ArgumentNullException(nameof(upper));

        Order(ref lower, ref upper);

        if (typeof(T) == typeof(string))
        {
            return (value as string).IsInRange(lower as string, upper as string);
        }

        var comparand = bounds.HasFlag(RangeBounds.IncludeLower) ? 0 : 1;
        if (value.CompareTo(lower) < comparand)
        {
            return false;
        }

        comparand = bounds.HasFlag(RangeBounds.IncludeUpper) ? 0 : -1;
        if (value.CompareTo(upper) > comparand)
        {
            return false;
        }

        return true;
    }

    private static void Order<T>(ref T lower, ref T upper) where T : IComparable<T>
    {
        if (lower.CompareTo(upper) == 1)
        {
            var temp = lower;
            lower = upper;
            upper = temp;
        }
    }
}