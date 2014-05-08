using System;

namespace Handyman
{
    public static class TypeExtensions
    {
        public static bool CanCastTo<T>(this Type type)
        {
            return type.CanCastTo(typeof (T));
        }

        public static bool CanCastTo(this Type type, Type targetType)
        {
            throw new NotImplementedException();
        }
    }
}