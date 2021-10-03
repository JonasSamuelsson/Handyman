using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Validation
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
            yield return new object[] { typeof(Flags), new Enum("Zero", "One", "Two") { Flags = true } };
            yield return new object[]
            {
                typeof(Flags), new Enum(new Dictionary<int, string>
                {
                    { 0, "Zero" },
                    { 1, "One" },
                    { 2, "Two" },
                }) { Flags = true }
            };

            yield return new object[] { typeof(Flags?), new Enum(0, 1, 2) { Flags = true, Nullable = true } };
            yield return new object[] { typeof(Flags?), new Enum("Zero", "One", "Two") { Flags = true, Nullable = true } };
            yield return new object[]
            {
                typeof(Flags?), new Enum(new Dictionary<int, string>
                {
                    { 0, "Zero" },
                    { 1, "One" },
                    { 2, "Two" },
                }) { Flags = true, Nullable = true}
            };

            yield return new object[] { typeof(Number), new Enum(0, 1, 2) };
            yield return new object[] { typeof(Number), new Enum("Zero", "One", "Two") };
            yield return new object[]
            {
                typeof(Number), new Enum(new Dictionary<int, string>
                {
                    { 0, "Zero" },
                    { 1, "One" },
                    { 2, "Two" },
                })
            };

            yield return new object[] { typeof(Number?), new Enum(0, 1, 2) { Nullable = true } };
            yield return new object[] { typeof(Number?), new Enum("Zero", "One", "Two") { Nullable = true } };
            yield return new object[]
            {
                typeof(Number?), new Enum(new Dictionary<int, string>
                {
                    { 0, "Zero" },
                    { 1, "One" },
                    { 2, "Two" },
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

            yield return new object[] { typeof(Number), new Enum("Zero", "One") };
            yield return new object[] { typeof(Number), new Enum("Zero", "One", "Two", "three") };

            yield return new object[] { typeof(Number), new Enum(new Dictionary<int,string>
            {
                {0, "Zero"},
                {1, "One"},
            }) };
            yield return new object[] { typeof(Number), new Enum(new Dictionary<int,string>
            {
                {0, "Two"},
                {1, "One"},
                {2, "Zero"},
            }) };
            yield return new object[] { typeof(Number), new Enum(new Dictionary<int,string>
            {
                {0, "Zero"},
                {1, "One"},
                {2, "Two"},
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