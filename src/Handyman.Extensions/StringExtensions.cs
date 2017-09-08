using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.Extensions
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> strings)
        {
            return string.Join(string.Empty, strings);
        }

        public static string Join(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings);
        }

        public static bool Contains(this string s, string value, StringComparison comparison)
        {
            return s.IndexOf(value, comparison) != -1;
        }

        public static bool IsEmpty(this string s)
        {
            return !s.IsNull() && s.Length == 0;
        }

        public static bool IsWhiteSpace(this string s)
        {
            return !s.IsNull() && s.IsNullOrWhiteSpace();
        }

        public static bool IsNull(this string s)
        {
            return s == null;
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static bool IsNotEmpty(this string s)
        {
            return !s.IsEmpty();
        }

        public static bool IsNotWhiteSpace(this string s)
        {
            return !s.IsWhiteSpace();
        }

        public static bool IsNotNull(this string s)
        {
            return !s.IsNull();
        }

        public static bool IsNotNullOrEmpty(this string s)
        {
            return !s.IsNullOrEmpty();
        }

        public static bool IsNotNullOrWhiteSpace(this string s)
        {
            return !s.IsNullOrWhiteSpace();
        }

        public static string SubstringSafe(this string s, int startIndex)
        {
            return s.SubstringSafe(startIndex, int.MaxValue);
        }

        public static string SubstringSafe(this string s, int startIndex, int length)
        {
            if (startIndex > s.Length) return string.Empty;
            length = Math.Min(length, s.Length - startIndex);
            return s.Substring(startIndex, length);
        }

        public static string Reverse(this string s)
        {
            return new string(s.ToCharArray().Reverse().ToArray());
        }

        public static T ToEnum<T>(this string s, IgnoreCase? ignoreCase = null) where T : struct
        {
            T value;
            if (!s.TryToEnum(ignoreCase ?? IgnoreCase.No, out value)) throw new ArgumentException();
            return value;
        }

        public static T? ToEnumOrNull<T>(this string s, IgnoreCase? ignoreCase = null) where T : struct
        {
            T value;
            return s.TryToEnum(ignoreCase ?? IgnoreCase.No, out value)
                       ? value
                       : default(T?);
        }

        public static T ToEnumOrDefault<T>(this string s, T @default, IgnoreCase? ignoreCase = null) where T : struct
        {
            return s.ToEnumOrDefault(() => @default);
        }

        public static T ToEnumOrDefault<T>(this string s, Func<T> factory, IgnoreCase? ignoreCase = null) where T : struct
        {
            return s.ToEnumOrNull<T>() ?? factory();
        }

        public static string IfNull(this string s, string @default)
        {
            return s.IfNull(() => @default);
        }

        public static string IfNull(this string s, Func<string> factory)
        {
            return s.IsNull() ? factory() : s;
        }

        public static string IfEmpty(this string s, string @default)
        {
            return s.IfEmpty(() => @default);
        }

        public static string IfEmpty(this string s, Func<string> factory)
        {
            return s.IsEmpty() ? factory() : s;
        }

        public static string IfWhiteSpace(this string s, string @default)
        {
            return s.IfWhiteSpace(() => @default);
        }

        public static string IfWhiteSpace(this string s, Func<string> factory)
        {
            return s.IsWhiteSpace() ? factory() : s;
        }

        public static string IfNullOrEmpty(this string s, string @default)
        {
            return s.IfNullOrEmpty(() => @default);
        }

        public static string IfNullOrEmpty(this string s, Func<string> factory)
        {
            return s.IsNullOrEmpty() ? factory() : s;
        }

        public static string IfNullOrWhiteSpace(this string s, string @default)
        {
            return s.IfNullOrWhiteSpace(() => @default);
        }

        public static string IfNullOrWhiteSpace(this string s, Func<string> factory)
        {
            return s.IsNullOrWhiteSpace() ? factory() : s;
        }

        public static bool TryToEnum<T>(this string s, out T value) where T : struct
        {
            return s.TryToEnum(IgnoreCase.No, out value);
        }

        public static bool TryToEnum<T>(this string s, IgnoreCase ignoreCase, out T value) where T : struct
        {
            var type = typeof(T);
            if (!type.GetTypeInfo().IsEnum) throw new ArgumentException();
            // ReSharper disable once RedundantAssignment
            value = default(T);
            return Enum.TryParse(s, ignoreCase == IgnoreCase.Yes, out value);
        }

        public static string IfNullGetEmpty(this string s)
        {
            return s ?? string.Empty;
        }
    }
}