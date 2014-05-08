namespace Handyman
{
    public static class BoolExtensions
    {
        public static bool IsFalse(this bool b)
        {
            return !b.IsTrue();
        }
    
        public static bool IsTrue(this bool b)
        {
            return b;
        }
    }
}