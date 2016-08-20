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

            Order(ref lower, ref upper);

            return value.CompareTo(lower) == -1
                ? lower
                : (upper.CompareTo(value) == -1
                    ? upper
                    : value);
        }

        public static bool IsInRange<T>(this T value, T lower, T upper) where T : IComparable<T>
        {
            Order(ref lower, ref upper);

            return typeof(T) == typeof(string)
                       ? StringExtensions.IsInRange(value as string, lower as string, upper as string)
                       : lower.CompareTo(value) <= 0 && value.CompareTo(upper) <= 0;
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
}