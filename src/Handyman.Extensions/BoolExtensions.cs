namespace Handyman.Extensions
{
    public static class BoolExtensions
    {
        public static bool IsFalse(this bool value)
        {
            return value == false;
        }
    
        public static bool IsTrue(this bool value)
        {
            return value;
        }

        public static bool Not(this bool value)
        {
            return !value;
        }
    }
}