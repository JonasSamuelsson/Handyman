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
            yield return new object[]
            {
                typeof(Flags), new Enum
                {
                    Flags = true,
                    Nullable = false,
                    Values =
                    {
                        { 0, "Zero" },
                        { 1, "One" },
                        { 2, "Two" },
                    }
                }
            };

            yield return new object[]
            {
                typeof(Flags?), new Enum
                {
                    Flags = true,
                    Nullable = true,
                    Values =
                    {
                        { 0, "Zero" },
                        { 1, "One" },
                        { 2, "Two" },
                    }
                }
            };

            yield return new object[]
            {
                typeof(Number), new Enum
                {
                    Flags = false,
                    Nullable = false,
                    Values =
                    {
                        { 0, "Zero" },
                        { 1, "One" },
                        { 2, "Two" },
                    }
                }
            };

            yield return new object[]
            {
                typeof(Number?), new Enum
                {
                    Flags = false,
                    Nullable = true,
                    Values =
                    {
                        { 0, "Zero" },
                        { 1, "One" },
                        { 2, "Two" },
                    }
                }
            };
        }

        [Theory, MemberData(nameof(ShouldFailWhenTypesDoNotMatchParams))]
        public void ShouldFailWhenTypesDoNotMatch(Type type, object dataContract)
        {
            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeFalse();
        }

        public static IEnumerable<object[]> ShouldFailWhenTypesDoNotMatchParams()
        {
            var values = new Dictionary<long, string>
            {
                { 0, "Zero" },
                { 1, "One" },
                { 2, "Two" },
            };

            yield return new object[] { typeof(int), new Enum() };

            yield return new object[] { typeof(Flags), new Enum { Flags = false, Nullable = false, Values = values } };
            yield return new object[] { typeof(Flags), new Enum { Flags = true, Nullable = true, Values = values } };

            yield return new object[] { typeof(Flags?), new Enum { Flags = false, Nullable = true, Values = values } };
            yield return new object[] { typeof(Flags?), new Enum { Flags = true, Nullable = false, Values = values } };

            yield return new object[] { typeof(Number), new Enum { Flags = false, Nullable = true, Values = values } };
            yield return new object[] { typeof(Number), new Enum { Flags = true, Nullable = false, Values = values } };

            yield return new object[] { typeof(Number?), new Enum { Flags = false, Nullable = false, Values = values } };
            yield return new object[] { typeof(Number?), new Enum { Flags = true, Nullable = true, Values = values } };
        }

        [Theory, MemberData(nameof(ShouldFailWhenValuesDoNotMatchParams))]
        public void ShouldFailWhenValuesDoNotMatch(Type type, object dataContract)
        {
            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeFalse();
        }

        public static IEnumerable<object[]> ShouldFailWhenValuesDoNotMatchParams()
        {
            yield return new object[]
            {
                typeof(Number), new Enum
                {
                    Flags = false,
                    Nullable = false,
                    Values =
                    {
                        { 0, "Zero" },
                        { 1, "One" }
                    }
                }
            };

            yield return new object[]
            {
                typeof(Number), new Enum
                {
                    Flags = false,
                    Nullable = false,
                    Values =
                    {
                        { 0, "Zero" },
                        { 1, "One" },
                        { 2, "Two" },
                        { 3, "Three" }
                    }
                }
            };
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