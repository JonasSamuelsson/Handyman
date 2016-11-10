using System;

namespace Handyman.Extensions
{
    public static class NullableExtensions
    {
        public static T GetValueOrDefault<T>(this T? nullable, T @default) where T : struct
        {
            return nullable.GetValueOrDefault(() => @default);
        }

        public static T GetValueOrDefault<T>(this T? nullable, Func<T> factory) where T : struct
        {
            return nullable.HasValue ? nullable.Value : factory();
        }

        public static T GetValueOrThrow<T>(this T? nullable) where T : struct
        {
            // ReSharper disable once PossibleInvalidOperationException
            return nullable.Value;
        }

        public static bool IsNull<T>(this T? nullable) where T : struct
        {
            return !nullable.HasValue;
        }

        public static bool IsNotNull<T>(this T? nullable) where T : struct
        {
            return nullable.HasValue;
        }
    }
}