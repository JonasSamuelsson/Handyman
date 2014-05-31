using System;

namespace Handyman
{
    public static class ComparableExtensions
    {
        public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable
        {
            return value.CompareTo(min) == 1 && value.CompareTo(max) == -1;
        }

        public static bool IsInRange<T>(this T value, T min, T max) where T : IComparable
        {
            return value.CompareTo(min) != -1 && value.CompareTo(max) != 1;
        }
    }
}