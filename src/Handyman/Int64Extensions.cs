namespace Handyman
{
    public static class Int64Extensions
    {
        public static bool IsBetween(this long number, long min, long max)
        {
            return min < number && number < max;
        }

        public static bool IsInRange(this long number, long min, long max)
        {
            return min <= number && number <= max;
        }
    }
}