using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings);
        }

        public static bool Contains(this string s, string value, StringComparison comparison)
        {
            return s.IndexOf(value, comparison) != -1;
        }

        public static bool ContainsWildcard(this string s, string pattern)
        {
            return s.ContainsWildcard(pattern, StringComparison.CurrentCulture);
        }

        public static bool ContainsWildcard(this string s, string pattern, StringComparison comparison)
        {
            var values = pattern.Split(new[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
            var startIndex = 0;
            foreach (var value in values)
            {
                if (s.IndexOf(value, startIndex, comparison) == -1) return false;
                startIndex += value.Length;
            }
            return true;
        }

        public static bool ContainsAll(this string s, params string[] values)
        {
            return s.ContainsAll(StringComparison.CurrentCulture, values);
        }

        public static bool ContainsAll(this string s, StringComparison comparison, params string[] values)
        {
            return values.All(x => s.Contains(x, comparison));
        }

        public static bool ContainsAny(this string s, params string[] values)
        {
            return s.ContainsAny(StringComparison.CurrentCulture, values);
        }

        public static bool ContainsAny(this string s, StringComparison comparison, params string[] values)
        {
            return values.Any(x => s.Contains(x, comparison));
        }

        public static bool EqualsWildcard(this string s, string value)
        {
            return s.Equals(value, StringComparison.CurrentCulture);
        }

        public static bool EqualsWildcard(this string s, string value, StringComparison comparison)
        {
            if (value.StartsWith("*")) return s.ContainsWildcard(value, comparison);
            var values = value.Split(new[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
            var first = values.First();
            if (!s.StartsWith(first, comparison)) return false;
            s = s.SubstringSafe(first.Length);
            value = values.Skip(1).Join("*");
            return s.ContainsWildcard(value, comparison);
        }

        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static bool IsMatch(this string s, string pattern)
        {
            return s.IsMatch(pattern, RegexOptions.None);
        }

        public static bool IsMatch(this string s, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(s, pattern, options);
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
            return (T)Enum.Parse(typeof(T), s, ignoreCase.GetValueOrDefault(IgnoreCase.No) == IgnoreCase.Yes);
        }

        public static T? ToEnumOrNull<T>(this string s, IgnoreCase? ignoreCase = null) where T : struct
        {
            T value;
            return Enum.TryParse(s, ignoreCase.GetValueOrDefault(IgnoreCase.No) == IgnoreCase.Yes, out value)
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
    }
}