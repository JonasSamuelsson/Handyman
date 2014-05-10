namespace Handyman
{
    public static class DecimalExtensions
    {
        public static bool IsBetween(this decimal number, decimal min, decimal max)
        {
            return min < number && number < max;
        }

        public static bool IsInRange(this decimal number, decimal min, decimal max)
        {
            return min <= number && number <= max;
        }
    }
}