using System;
using JetBrains.Annotations;

namespace Handyman
{
    public static class ComparableExtensions
    {
        public static T Clamp<T>([NotNull] this T value, [NotNull] T lower, [NotNull] T upper) where T : IComparable<T>
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (lower == null) throw new ArgumentNullException(nameof(lower));
            if (upper == null) throw new ArgumentNullException(nameof(upper));

            if (lower.CompareTo(upper) == 1)
            {
                var temp = lower;
                lower = upper;
                upper = temp;
            }

            return value.CompareTo(lower) == -1
                ? lower
                : (upper.CompareTo(value) == -1
                    ? upper
                    : value);
        }

        public static bool IsInRange<T>(this T value, T min, T max) where T : IComparable
        {
            return typeof(T) == typeof(string)
                       ? StringExtensions.IsInRange(value as string, min as string, max as string)
                       : min.CompareTo(value) <= 0 && value.CompareTo(max) <= 0;
        }
    }
}