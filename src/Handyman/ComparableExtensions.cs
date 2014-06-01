using System;

namespace Handyman
{
    public static class ComparableExtensions
    {
        public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable
        {
            return typeof(T) == typeof(string)
                       ? StringExtensions.IsBetween(value as string, min as string, max as string)
                       : min.CompareTo(value) < 0 && value.CompareTo(max) < 0;
        }

        public static bool IsInRange<T>(this T value, T min, T max) where T : IComparable
        {
            return typeof(T) == typeof(string)
                       ? StringExtensions.IsInRange(value as string, min as string, max as string)
                       : min.CompareTo(value) <= 0 && value.CompareTo(max) <= 0;
        }
    }
}