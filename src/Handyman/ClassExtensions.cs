namespace Handyman
{
    public static class ClassExtensions
    {
        public static bool IsNull<T>(this T instance) where T : class
        {
            return instance == null;
        }

        public static bool IsNotNull<T>(this T instance) where T : class
        {
            return !instance.IsNull();
        }
    }
}