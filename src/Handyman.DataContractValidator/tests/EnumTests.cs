using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class EnumTests
    {
        [Theory, MemberData(nameof(ShouldPassWhenTypesMatchParams))]
        public void ShouldPassWhenTypesMatch(Type type, object dataContract)
        {
            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
        }

        public static IEnumerable<object[]> ShouldPassWhenTypesMatchParams()
        {
            yield return new object[] { typeof(Flags), new Enum(0, 1, 2) { Flags = true } };
            yield return new object[] { typeof(Flags), new Enum("zero", "one", "two") { Flags = true } };
            yield return new object[]
            {
                typeof(Flags), new Enum(new Dictionary<int, string>
                {
                    { 0, "zero" },
                    { 1, "one" },
                    { 2, "two" },
                }) { Flags = true }
            };

            yield return new object[] { typeof(Flags?), new Enum(0, 1, 2) { Flags = true, Nullable = true } };
            yield return new object[] { typeof(Flags?), new Enum("zero", "one", "two") { Flags = true, Nullable = true } };
            yield return new object[]
            {
                typeof(Flags?), new Enum(new Dictionary<int, string>
                {
                    { 0, "zero" },
                    { 1, "one" },
                    { 2, "two" },
                }) { Flags = true, Nullable = true}
            };

            yield return new object[] { typeof(Number), new Enum(0, 1, 2) };
            yield return new object[] { typeof(Number), new Enum("zero", "one", "two") };
            yield return new object[]
            {
                typeof(Number), new Enum(new Dictionary<int, string>
                {
                    { 0, "zero" },
                    { 1, "one" },
                    { 2, "two" },
                })
            };

            yield return new object[] { typeof(Number?), new Enum(0, 1, 2) { Nullable = true } };
            yield return new object[] { typeof(Number?), new Enum("zero", "one", "two") { Nullable = true } };
            yield return new object[]
            {
                typeof(Number?), new Enum(new Dictionary<int, string>
                {
                    { 0, "zero" },
                    { 1, "one" },
                    { 2, "two" },
                }) { Nullable = true}
            };
        }

        [Theory, MemberData(nameof(ShouldFailWhenTypesDoNotMatchParams))]
        public void ShouldFailWhenTypesDoNotMatch(Type type, object dataContract)
        {
            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeFalse();
        }

        public static IEnumerable<object[]> ShouldFailWhenTypesDoNotMatchParams()
        {
            yield return new object[] { typeof(int), new Enum(0) };

            yield return new object[] { typeof(Flags), new Enum(0, 1, 2) };
            yield return new object[] { typeof(Flags), new Enum(0, 1, 2) { Flags = true, Nullable = true } };

            yield return new object[] { typeof(Flags?), new Enum(0, 1, 2) { Nullable = true } };
            yield return new object[] { typeof(Flags?), new Enum(0, 1, 2) { Flags = true } };

            yield return new object[] { typeof(Number), new Enum(0, 1, 2) { Flags = true } };
            yield return new object[] { typeof(Number), new Enum(0, 1, 2) { Nullable = true } };

            yield return new object[] { typeof(Number?), new Enum(0, 1, 2) { Flags = true, Nullable = true } };
            yield return new object[] { typeof(Number?), new Enum(0, 1, 2) };
        }

        [Theory, MemberData(nameof(ShouldFailWhenValuesDoNotMatchParams))]
        public void ShouldFailWhenValuesDoNotMatch(Type type, object dataContract)
        {
            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeFalse();
        }

        public static IEnumerable<object[]> ShouldFailWhenValuesDoNotMatchParams()
        {
            yield return new object[] { typeof(Number), new Enum(0, 1) };
            yield return new object[] { typeof(Number), new Enum(0, 1, 2, 3) };

            yield return new object[] { typeof(Number), new Enum("zero", "one") };
            yield return new object[] { typeof(Number), new Enum("zero", "one", "two", "three") };

            yield return new object[] { typeof(Number), new Enum(new Dictionary<int,string>
            {
                {0, "zero"},
                {1, "one"},
            }) };
            yield return new object[] { typeof(Number), new Enum(new Dictionary<int,string>
            {
                {0, "two"},
                {1, "one"},
                {2, "zero"},
            }) };
            yield return new object[] { typeof(Number), new Enum(new Dictionary<int,string>
            {
                {0, "zero"},
                {1, "one"},
                {2, "two"},
                {3, "three"},
            }) };

        }

        [Flags]
        private enum Flags
        {
            Zero = 0,
            One = 1,
            Two = 2
        }

        private enum Number
        {
            Zero = 0,
            One = 1,
            Two = 2
        }
    }
}