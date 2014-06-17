using Shouldly;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Handyman.Tests
{
    public class StringExtensionsTests
    {
        public void ShouldJoinStrings()
        {
            new[] { "join", "multiple", "strings" }.Join(" ").ShouldBe("join multiple strings");
        }

        public void ShouldFormatString()
        {
            "{0} {1}!".FormatWith("Hello", "world").ShouldBe("Hello world!");
        }

        public void ShouldCheckIfStringEquals()
        {
            "Encyclopædia".EqualsString("encyclopaedia", CultureInfo.InvariantCulture).ShouldBe(false);
            "Encyclopædia".EqualsString("encyclopaedia", CultureInfo.InvariantCulture, IgnoreCase.Yes).ShouldBe(true);
        }

        public void ShouldCheckIfStringEqualsWildcard()
        {
            "Hello".EqualsWildcard("*e*o").ShouldBe(false);
            "Hello".EqualsWildcard("*e*o", StringComparison.InvariantCultureIgnoreCase).ShouldBe(true);
        }

        public void ShouldCheckIfStringContainsValue()
        {
            "Hello World".Contains("lo wo", StringComparison.InvariantCulture).ShouldBe(false);
            "Hello World".Contains("lo wo", StringComparison.InvariantCultureIgnoreCase).ShouldBe(true);
            "Hello World".Contains("lo wo", CultureInfo.InvariantCulture).ShouldBe(false);
            "Hello World".Contains("lo wo", CultureInfo.InvariantCulture, CompareOptions.IgnoreCase).ShouldBe(true);

        }

        public void ShouldCheckIfStringContainsWildcardValue()
        {
            "ohe".ContainsWildcard("he*o").ShouldBe(false);
            "Hello world".ContainsWildcard("he*o").ShouldBe(false);
            "Hello world".ContainsWildcard("he*o", StringComparison.InvariantCultureIgnoreCase).ShouldBe(true);
        }

        public void ShouldCheckIfStringIsMatch()
        {
            "Hello world".IsMatch("^hello").ShouldBe(false);
            "Hello world".IsMatch("^hello", RegexOptions.IgnoreCase).ShouldBe(true);
        }

        public void ShouldCheckIfStringIsNull()
        {
            ((string)null).IsNull().ShouldBe(true);
            "".IsNull().ShouldBe(false);
        }

        public void ShouldCheckIfStringIsEmpty()
        {
            ((string)null).IsEmpty().ShouldBe(false);
            "".IsEmpty().ShouldBe(true);
            " ".IsEmpty().ShouldBe(false);
            "x".IsEmpty().ShouldBe(false);
        }

        public void ShouldCheckIfStringIsWhiteSpace()
        {
            ((string)null).IsWhiteSpace().ShouldBe(false);
            "".IsWhiteSpace().ShouldBe(true);
            " ".IsWhiteSpace().ShouldBe(true);
            "x".IsWhiteSpace().ShouldBe(false);
        }

        public void ShouldCheckIfStringIsNullOrEmpty()
        {
            ((string)null).IsNullOrEmpty().ShouldBe(true);
            "".IsNullOrEmpty().ShouldBe(true);
            " ".IsNullOrEmpty().ShouldBe(false);
            "x".IsNullOrEmpty().ShouldBe(false);
        }

        public void ShouldCheckIfStringIsNullOrWhiteSpace()
        {
            ((string)null).IsNullOrWhiteSpace().ShouldBe(true);
            "".IsNullOrWhiteSpace().ShouldBe(true);
            " ".IsNullOrWhiteSpace().ShouldBe(true);
            "x".IsNullOrWhiteSpace().ShouldBe(false);
        }

        public void ShouldCheckIfStringIsNotNull()
        {
            ((string)null).IsNotNull().ShouldBe(false);
            "".IsNotNull().ShouldBe(true);
        }

        public void ShouldCheckIfStringIsNotEmpty()
        {
            ((string)null).IsNotEmpty().ShouldBe(true);
            "".IsNotEmpty().ShouldBe(false);
            " ".IsNotEmpty().ShouldBe(true);
            "x".IsNotEmpty().ShouldBe(true);
        }

        public void ShouldCheckIfStringIsNotWhiteSpace()
        {
            ((string)null).IsNotWhiteSpace().ShouldBe(true);
            "".IsNotWhiteSpace().ShouldBe(false);
            " ".IsNotWhiteSpace().ShouldBe(false);
            "x".IsNotWhiteSpace().ShouldBe(true);
        }

        public void ShouldCheckIfStringIsNotNullOrEmpty()
        {
            ((string)null).IsNotNullOrEmpty().ShouldBe(false);
            "".IsNotNullOrEmpty().ShouldBe(false);
            " ".IsNotNullOrEmpty().ShouldBe(true);
            "x".IsNotNullOrEmpty().ShouldBe(true);
        }

        public void ShouldCheckIfStringIsNotNullOrWhiteSpace()
        {
            ((string)null).IsNotNullOrWhiteSpace().ShouldBe(false);
            "".IsNotNullOrWhiteSpace().ShouldBe(false);
            " ".IsNotNullOrWhiteSpace().ShouldBe(false);
            "x".IsNotNullOrWhiteSpace().ShouldBe(true);
        }

        public void ShouldGetSubstring()
        {
            "".SubstringSafe(1).ShouldBe(string.Empty);
            "".SubstringSafe(1, 1).ShouldBe(string.Empty);
            "Hello".SubstringSafe(1).ShouldBe("ello");
            "Hello".SubstringSafe(1, 1).ShouldBe("e");
        }

        public void ShouldReverseTheString()
        {
            "Hello".Reverse().ShouldBe("olleH");
        }

        public void ShouldConvertStringToEnumOrThrow()
        {
            Should.Throw<ArgumentException>(() => "yes".ToEnum<IgnoreCase>());
            "yes".ToEnum<IgnoreCase>(IgnoreCase.Yes).ShouldBe(IgnoreCase.Yes);
        }

        public void ShouldConvertStringToEnumOrNull()
        {
            "yes".ToEnumOrNull<IgnoreCase>().ShouldBe(null);
            "yes".ToEnumOrNull<IgnoreCase>(IgnoreCase.Yes).ShouldBe(IgnoreCase.Yes);
        }

        public void ShouldConvertStringToEnumOrDefault()
        {
            "yes".ToEnumOrDefault(IgnoreCase.No).ShouldBe(IgnoreCase.No);
            "yes".ToEnumOrDefault(() => IgnoreCase.No).ShouldBe(IgnoreCase.No);
        }

        public void ShouldGetValueIfNull()
        {
            ((string)null).IfNull("value").ShouldBe("value");
            ((string)null).IfNull(() => "value").ShouldBe("value");
            "".IfNull("value").ShouldBe("");
            "".IfNull(() => "value").ShouldBe("");
            " ".IfNull("value").ShouldBe(" ");
            " ".IfNull(() => "value").ShouldBe(" ");
            "foobar".IfNull("value").ShouldBe("foobar");
            "foobar".IfNull(() => "value").ShouldBe("foobar");
        }

        public void ShouldGetValueIfEmpty()
        {
            ((string)null).IfEmpty("value").ShouldBe(null);
            ((string)null).IfEmpty(() => "value").ShouldBe(null);
            "".IfEmpty("value").ShouldBe("value");
            "".IfEmpty(() => "value").ShouldBe("value");
            " ".IfEmpty("value").ShouldBe(" ");
            " ".IfEmpty(() => "value").ShouldBe(" ");
            "foobar".IfEmpty("value").ShouldBe("foobar");
            "foobar".IfEmpty(() => "value").ShouldBe("foobar");
        }

        public void ShouldGetValueIfWhiteSpace()
        {
            ((string)null).IfWhiteSpace("value").ShouldBe(null);
            ((string)null).IfWhiteSpace(() => "value").ShouldBe(null);
            "".IfWhiteSpace("value").ShouldBe("value");
            "".IfWhiteSpace(() => "value").ShouldBe("value");
            " ".IfWhiteSpace("value").ShouldBe("value");
            " ".IfWhiteSpace(() => "value").ShouldBe("value");
            "foobar".IfWhiteSpace("value").ShouldBe("foobar");
            "foobar".IfWhiteSpace(() => "value").ShouldBe("foobar");
        }

        public void ShouldGetValueIfNullOrEmpty()
        {
            ((string)null).IfNullOrEmpty("value").ShouldBe("value");
            ((string)null).IfNullOrEmpty(() => "value").ShouldBe("value");
            "".IfNullOrEmpty("value").ShouldBe("value");
            "".IfNullOrEmpty(() => "value").ShouldBe("value");
            " ".IfNullOrEmpty("value").ShouldBe(" ");
            " ".IfNullOrEmpty(() => "value").ShouldBe(" ");
            "foobar".IfNullOrEmpty("value").ShouldBe("foobar");
            "foobar".IfNullOrEmpty(() => "value").ShouldBe("foobar");
        }

        public void ShouldGetValueIfNullOrWhiteSpace()
        {
            ((string)null).IfNullOrWhiteSpace("value").ShouldBe("value");
            ((string)null).IfNullOrWhiteSpace(() => "value").ShouldBe("value");
            "".IfNullOrWhiteSpace("value").ShouldBe("value");
            "".IfNullOrWhiteSpace(() => "value").ShouldBe("value");
            " ".IfNullOrWhiteSpace("value").ShouldBe("value");
            " ".IfNullOrWhiteSpace(() => "value").ShouldBe("value");
            "foobar".IfNullOrWhiteSpace("value").ShouldBe("foobar");
            "foobar".IfNullOrWhiteSpace(() => "value").ShouldBe("foobar");
        }

        public void ShouldTryConvertStringToEnum()
        {
            int @int;
            Should.Throw<ArgumentException>(() => "one".TryToEnum(out @int));
            Should.Throw<ArgumentException>(() => "one".TryToEnum(IgnoreCase.Yes, out @int));

            // ReSharper disable once RedundantAssignment
            var number = Number.One;

            "one".TryToEnum(out number).ShouldBe(false);
            number.ShouldBe(Number.Zero);

            "one".TryToEnum(IgnoreCase.Yes, out number).ShouldBe(true);
            number.ShouldBe(Number.One);

            // ReSharper disable once RedundantAssignment
            number = Number.Zero;
            "1".TryToEnum(out number).ShouldBe(true);
            number.ShouldBe(Number.One);
        }

        private enum Number
        {
            Zero = 0,
            One = 1
        }

        public void TryToShort()
        {
            // ReSharper disable once RedundantAssignment
            short result = 1;
            "".TryToShort(out result).ShouldBe(false);
            result.ShouldBe((short)0);

            // ReSharper disable once RedundantAssignment
            result = 1;
            "".TryToShort(Configuration.FormatProvider(), out result).ShouldBe(false);
            result.ShouldBe((short)0);

            "1".TryToShort(out result).ShouldBe(true);
            result.ShouldBe((short)1);

            "2".TryToShort(Configuration.FormatProvider(), out result).ShouldBe(true);
            result.ShouldBe((short)2);
        }

        public void ToShort()
        {
            Should.Throw<ArgumentException>(() => "".ToShort());
            Should.Throw<ArgumentException>(() => "".ToShort(Configuration.FormatProvider()));

            "1".ToShort().ShouldBe((short)1);
            "2".ToShort(Configuration.FormatProvider()).ShouldBe((short)2);
        }

        public void ToShortOrDefault()
        {
            "one".ToShortOrDefault(0).ShouldBe((short)0);
            "two".ToShortOrDefault(() => 0).ShouldBe((short)0);
            "three".ToShortOrDefault(Configuration.FormatProvider(), 0).ShouldBe((short)0);
            "four".ToShortOrDefault(Configuration.FormatProvider(), () => 0).ShouldBe((short)0);

            "1".ToShortOrDefault(0).ShouldBe((short)1);
            "2".ToShortOrDefault(() => 0).ShouldBe((short)2);
            "3".ToShortOrDefault(Configuration.FormatProvider(), 0).ShouldBe((short)3);
            "4".ToShortOrDefault(Configuration.FormatProvider(), () => 0).ShouldBe((short)4);
        }

        public void ToShortOrZero()
        {
            "one".ToShortOrZero().ShouldBe((short)0);
            "two".ToShortOrZero(Configuration.FormatProvider()).ShouldBe((short)0);

            "1".ToShortOrZero().ShouldBe((short)1);
            "2".ToShortOrZero(Configuration.FormatProvider()).ShouldBe((short)2);
        }

        public void TryToInt()
        {
            // ReSharper disable once RedundantAssignment
            var result = 1;
            "".TryToInt(out result).ShouldBe(false);
            result.ShouldBe(0);

            // ReSharper disable once RedundantAssignment
            result = 1;
            "".TryToInt(Configuration.FormatProvider(), out result).ShouldBe(false);
            result.ShouldBe(0);

            "1".TryToInt(out result).ShouldBe(true);
            result.ShouldBe(1);

            "2".TryToInt(Configuration.FormatProvider(), out result).ShouldBe(true);
            result.ShouldBe(2);
        }

        public void ToInt()
        {
            Should.Throw<ArgumentException>(() => "".ToInt());
            Should.Throw<ArgumentException>(() => "".ToInt(Configuration.FormatProvider()));

            "1".ToInt().ShouldBe(1);
            "2".ToInt(Configuration.FormatProvider()).ShouldBe(2);
        }

        public void ToIntOrDefault()
        {
            "one".ToIntOrDefault(0).ShouldBe(0);
            "two".ToIntOrDefault(() => 0).ShouldBe(0);
            "three".ToIntOrDefault(Configuration.FormatProvider(), 0).ShouldBe(0);
            "four".ToIntOrDefault(Configuration.FormatProvider(), () => 0).ShouldBe(0);

            "1".ToIntOrDefault(0).ShouldBe(1);
            "2".ToIntOrDefault(() => 0).ShouldBe(2);
            "3".ToIntOrDefault(Configuration.FormatProvider(), 0).ShouldBe(3);
            "4".ToIntOrDefault(Configuration.FormatProvider(), () => 0).ShouldBe(4);
        }

        public void ToIntOrZero()
        {
            "one".ToIntOrZero().ShouldBe(0);
            "two".ToIntOrZero(Configuration.FormatProvider()).ShouldBe(0);

            "1".ToIntOrZero().ShouldBe(1);
            "2".ToIntOrZero(Configuration.FormatProvider()).ShouldBe(2);
        }

        public void TryToLong()
        {
            // ReSharper disable once RedundantAssignment
            var result = 1L;
            "".TryToLong(out result).ShouldBe(false);
            result.ShouldBe(0);

            // ReSharper disable once RedundantAssignment
            result = 1;
            "".TryToLong(Configuration.FormatProvider(), out result).ShouldBe(false);
            result.ShouldBe(0);

            "1".TryToLong(out result).ShouldBe(true);
            result.ShouldBe(1);

            "2".TryToLong(Configuration.FormatProvider(), out result).ShouldBe(true);
            result.ShouldBe(2);
        }

        public void ToLong()
        {
            Should.Throw<ArgumentException>(() => "".ToLong());
            Should.Throw<ArgumentException>(() => "".ToLong(Configuration.FormatProvider()));

            "1".ToLong().ShouldBe(1);
            "2".ToLong(Configuration.FormatProvider()).ShouldBe(2);
        }

        public void ToLongOrDefault()
        {
            "one".ToLongOrDefault(0).ShouldBe(0);
            "two".ToLongOrDefault(() => 0).ShouldBe(0);
            "three".ToLongOrDefault(Configuration.FormatProvider(), 0).ShouldBe(0);
            "four".ToLongOrDefault(Configuration.FormatProvider(), () => 0).ShouldBe(0);

            "1".ToLongOrDefault(0).ShouldBe(1);
            "2".ToLongOrDefault(() => 0).ShouldBe(2);
            "3".ToLongOrDefault(Configuration.FormatProvider(), 0).ShouldBe(3);
            "4".ToLongOrDefault(Configuration.FormatProvider(), () => 0).ShouldBe(4);
        }

        public void ToLongOrZero()
        {
            "one".ToLongOrZero().ShouldBe(0);
            "two".ToLongOrZero(Configuration.FormatProvider()).ShouldBe(0);

            "1".ToLongOrZero().ShouldBe(1);
            "2".ToLongOrZero(Configuration.FormatProvider()).ShouldBe(2);
        }

        public void ShouldCheckIfStringIsBetween()
        {
            Configuration.StringComparison = () => StringComparison.Ordinal;

            "a".IsBetween("b", "d").ShouldBe(false);
            "b".IsBetween("b", "d").ShouldBe(false);
            "c".IsBetween("b", "d").ShouldBe(true);
            "d".IsBetween("b", "d").ShouldBe(false);
            "e".IsBetween("b", "d").ShouldBe(false);

            "A".IsBetween("b", "d").ShouldBe(false);
            "B".IsBetween("b", "d").ShouldBe(false);
            "C".IsBetween("b", "d").ShouldBe(false);
            "D".IsBetween("b", "d").ShouldBe(false);
            "E".IsBetween("b", "d").ShouldBe(false);

            "A".IsBetween("b", "d", StringComparison.InvariantCultureIgnoreCase).ShouldBe(false);
            "B".IsBetween("b", "d", StringComparison.InvariantCultureIgnoreCase).ShouldBe(false);
            "C".IsBetween("b", "d", StringComparison.InvariantCultureIgnoreCase).ShouldBe(true);
            "D".IsBetween("b", "d", StringComparison.InvariantCultureIgnoreCase).ShouldBe(false);
            "E".IsBetween("b", "d", StringComparison.InvariantCultureIgnoreCase).ShouldBe(false);

            "C".IsBetween("b", "d", CultureInfo.InvariantCulture).ShouldBe(true);
            "C".IsBetween("b", "d", CultureInfo.InvariantCulture, CompareOptions.Ordinal).ShouldBe(false);
        }

        public void ShouldCheckIfStringIsInRange()
        {
            Configuration.StringComparison = () => StringComparison.Ordinal;

            "a".IsInRange("b", "d").ShouldBe(false);
            "b".IsInRange("b", "d").ShouldBe(true);
            "c".IsInRange("b", "d").ShouldBe(true);
            "d".IsInRange("b", "d").ShouldBe(true);
            "e".IsInRange("b", "d").ShouldBe(false);

            "A".IsInRange("b", "d").ShouldBe(false);
            "B".IsInRange("b", "d").ShouldBe(false);
            "C".IsInRange("b", "d").ShouldBe(false);
            "D".IsInRange("b", "d").ShouldBe(false);
            "E".IsInRange("b", "d").ShouldBe(false);

            "A".IsInRange("b", "d", StringComparison.InvariantCultureIgnoreCase).ShouldBe(false);
            "B".IsInRange("b", "d", StringComparison.InvariantCultureIgnoreCase).ShouldBe(true);
            "C".IsInRange("b", "d", StringComparison.InvariantCultureIgnoreCase).ShouldBe(true);
            "D".IsInRange("b", "d", StringComparison.InvariantCultureIgnoreCase).ShouldBe(true);
            "E".IsInRange("b", "d", StringComparison.InvariantCultureIgnoreCase).ShouldBe(false);

            "C".IsInRange("b", "d", CultureInfo.InvariantCulture).ShouldBe(true);
            "C".IsInRange("b", "d", CultureInfo.InvariantCulture, CompareOptions.Ordinal).ShouldBe(false);
        }

        [Culture("en-US")]
        public void ShouldTryConvertStringToDouble()
        {
            double result;
            "1,1".TryToDouble(out result).ShouldBe(false);
            result.ShouldBe(0);

            "2,2".TryToDouble(CultureInfo.InvariantCulture, out result).ShouldBe(false);
            result.ShouldBe(0);

            "3.3".TryToDouble(out result).ShouldBe(true);
            result.ShouldBe(3.3);

            "4.4".TryToDouble(out result).ShouldBe(true);
            result.ShouldBe(4.4);
        }

        [Culture("en-US")]
        public void ShouldConvertToDoubleOrThrow()
        {
            Should.Throw<FormatException>(() => "1,1".ToDouble());
            Should.Throw<FormatException>(() => "2,2".ToDouble(CultureInfo.InvariantCulture));

            "3.3".ToDouble().ShouldBe(3.3);
            "4.4".ToDouble(CultureInfo.InvariantCulture).ShouldBe(4.4);
        }

        [Culture("en-US")]
        public void ShouldConvertToDoubleOrNull()
        {
            "1,1".ToDoubleOrNull().ShouldBe(null);
            "2,2".ToDoubleOrNull(CultureInfo.InvariantCulture).ShouldBe(null);

            "3.3".ToDoubleOrNull().ShouldBe(3.3);
            "4.4".ToDoubleOrNull(CultureInfo.InvariantCulture).ShouldBe(4.4);
        }

        [Culture("en-US")]
        public void ShouldConvertToDoubleOrDefault()
        {
            "1,1".ToDoubleOrDefault(0).ShouldBe(0);
            "1,1".ToDoubleOrDefault(() => 0).ShouldBe(0);

            "2,2".ToDoubleOrDefault(CultureInfo.InvariantCulture, 0).ShouldBe(0);
            "2,2".ToDoubleOrDefault(CultureInfo.InvariantCulture, () => 0).ShouldBe(0);

            "3.3".ToDoubleOrDefault(0).ShouldBe(3.3);
            "3.3".ToDoubleOrDefault(() => 0).ShouldBe(3.3);

            "4.4".ToDoubleOrDefault(CultureInfo.InvariantCulture, 0).ShouldBe(4.4);
            "4.4".ToDoubleOrDefault(CultureInfo.InvariantCulture, () => 0).ShouldBe(4.4);
        }

        [Culture("en-US")]
        public void ShouldConvertToDoubleOrZero()
        {
            "1,1".ToDoubleOrZero().ShouldBe(0);
            "2,2".ToDoubleOrZero(CultureInfo.InvariantCulture).ShouldBe(0);

            "3.3".ToDoubleOrZero().ShouldBe(3.3);
            "4.4".ToDoubleOrZero(CultureInfo.InvariantCulture).ShouldBe(4.4);
        }
    }
}