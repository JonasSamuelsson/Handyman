using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.Extensions
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings);
        }

        public static bool Contains(this string s, string value, StringComparison comparison)
        {
            return s.Contains(value, (x, y) => x.IndexOf(y, comparison));
        }

        public static bool Contains(this string s, string value, CultureInfo cultureInfo)
        {
            return s.Contains(value, cultureInfo, CompareOptions.None);
        }

        public static bool Contains(this string s, string value, CultureInfo cultureInfo, CompareOptions options)
        {
            return s.Contains(value, (x, y) => cultureInfo.CompareInfo.IndexOf(s, value, options));
        }

        private static bool Contains(this string s, string value, Comparison<string> comparer)
        {
            return comparer(s, value) != -1;
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

        public static bool EqualsString(this string s, string value, CultureInfo cultureInfo)
        {
            return s.EqualsString(value, cultureInfo, IgnoreCase.No);
        }

        public static bool EqualsString(this string s, string value, CultureInfo cultureInfo, IgnoreCase ignoreCase)
        {
            return cultureInfo.CompareInfo.Compare(s, value, ignoreCase == IgnoreCase.Yes ? CompareOptions.IgnoreCase : CompareOptions.None) == 0;
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
            if (!type.IsEnum) throw new ArgumentException();
            // ReSharper disable once RedundantAssignment
            value = default(T);
            return Enum.TryParse(s, ignoreCase == IgnoreCase.Yes, out value);
        }

        public static bool TryToShort(this string s, out short result)
        {
            return s.TryToShort(Configuration.FormatProvider(), out result);
        }

        public static bool TryToShort(this string s, IFormatProvider formatProvider, out short result)
        {
            return short.TryParse(s, NumberStyles.Integer, formatProvider, out result);
        }

        public static short ToShort(this string s)
        {
            return s.ToShort(Configuration.FormatProvider());
        }

        public static short ToShort(this string s, IFormatProvider formatProvider)
        {
            short result;
            if (!s.TryToShort(formatProvider, out result)) throw new ArgumentException();
            return result;
        }

        public static short ToShortOrDefault(this string s, short @default)
        {
            return s.ToShortOrDefault(Configuration.FormatProvider(), () => @default);
        }

        public static short ToShortOrDefault(this string s, Func<short> factory)
        {
            return s.ToShortOrDefault(Configuration.FormatProvider(), factory());
        }

        public static short ToShortOrDefault(this string s, IFormatProvider formatProvider, short @default)
        {
            return s.ToShortOrDefault(formatProvider, () => @default);
        }

        public static short ToShortOrDefault(this string s, IFormatProvider formatProvider, Func<short> factory)
        {
            short result;
            return s.TryToShort(formatProvider, out result)
                       ? result
                       : factory();
        }

        public static short ToShortOrZero(this string s)
        {
            return s.ToShortOrZero(Configuration.FormatProvider());
        }

        public static short ToShortOrZero(this string s, IFormatProvider formatProvider)
        {
            return s.ToShortOrDefault(formatProvider, 0);
        }

        public static bool TryToInt(this string s, out int result)
        {
            return s.TryToInt(Configuration.FormatProvider(), out result);
        }

        public static bool TryToInt(this string s, IFormatProvider formatProvider, out int result)
        {
            return int.TryParse(s, NumberStyles.Integer, formatProvider, out result);
        }

        public static int ToInt(this string s)
        {
            return s.ToInt(Configuration.FormatProvider());
        }

        public static int ToInt(this string s, IFormatProvider formatProvider)
        {
            int result;
            if (!s.TryToInt(formatProvider, out result)) throw new ArgumentException();
            return result;
        }

        public static int ToIntOrDefault(this string s, int @default)
        {
            return s.ToIntOrDefault(Configuration.FormatProvider(), () => @default);
        }

        public static int ToIntOrDefault(this string s, Func<int> factory)
        {
            return s.ToIntOrDefault(Configuration.FormatProvider(), factory());
        }

        public static int ToIntOrDefault(this string s, IFormatProvider formatProvider, int @default)
        {
            return s.ToIntOrDefault(formatProvider, () => @default);
        }

        public static int ToIntOrDefault(this string s, IFormatProvider formatProvider, Func<int> factory)
        {
            int result;
            return s.TryToInt(formatProvider, out result)
                       ? result
                       : factory();
        }

        public static int ToIntOrZero(this string s)
        {
            return s.ToIntOrZero(Configuration.FormatProvider());
        }

        public static int ToIntOrZero(this string s, IFormatProvider formatProvider)
        {
            return s.ToIntOrDefault(formatProvider, 0);
        }

        public static bool TryToLong(this string s, out long result)
        {
            return s.TryToLong(Configuration.FormatProvider(), out result);
        }

        public static bool TryToLong(this string s, IFormatProvider formatProvider, out long result)
        {
            return long.TryParse(s, NumberStyles.Integer, formatProvider, out result);
        }

        public static long ToLong(this string s)
        {
            return s.ToLong(Configuration.FormatProvider());
        }

        public static long ToLong(this string s, IFormatProvider formatProvider)
        {
            long result;
            if (!s.TryToLong(formatProvider, out result)) throw new ArgumentException();
            return result;
        }

        public static long ToLongOrDefault(this string s, long @default)
        {
            return s.ToLongOrDefault(Configuration.FormatProvider(), () => @default);
        }

        public static long ToLongOrDefault(this string s, Func<long> factory)
        {
            return s.ToLongOrDefault(Configuration.FormatProvider(), factory());
        }

        public static long ToLongOrDefault(this string s, IFormatProvider formatProvider, long @default)
        {
            return s.ToLongOrDefault(formatProvider, () => @default);
        }

        public static long ToLongOrDefault(this string s, IFormatProvider formatProvider, Func<long> factory)
        {
            long result;
            return s.TryToLong(formatProvider, out result)
                       ? result
                       : factory();
        }

        public static long ToLongOrZero(this string s)
        {
            return s.ToLongOrZero(Configuration.FormatProvider());
        }

        public static long ToLongOrZero(this string s, IFormatProvider formatProvider)
        {
            return s.ToLongOrDefault(formatProvider, 0);
        }

        public static bool IsInRange(this string value, string min, string max)
        {
            return value.IsInRange(min, max, Configuration.StringComparison());
        }

        public static bool IsInRange(this string value, string min, string max, StringComparison comparison)
        {
            return string.Compare(min, value, comparison) <= 0 && string.Compare(value, max, comparison) <= 0;
        }

        public static bool IsInRange(this string value, string min, string max, CultureInfo cultureInfo, CompareOptions options = CompareOptions.None)
        {
            return string.Compare(min, value, cultureInfo, options) <= 0 && string.Compare(value, max, cultureInfo, options) <= 0;
        }
    }
}