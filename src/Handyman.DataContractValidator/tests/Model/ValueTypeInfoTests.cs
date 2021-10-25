using Handyman.DataContractValidator.Model;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Model
{
    public class ValueTypeInfoTests
    {
        [Theory, MemberData(nameof(ShouldProvideTypeNameForTypesWithTypeKeywordParams))]
        public void ShouldProvideTypeNameForTypesWithTypeKeyword(Type valueType, string expected)
        {
            new ValueTypeInfo
            {
                Type = valueType
            }.TypeName.ShouldBe(expected);
        }

        public static IEnumerable<object[]> ShouldProvideTypeNameForTypesWithTypeKeywordParams()
        {
            return new[]
            {
                new object[] { typeof(bool), "bool" },
                new object[] { typeof(decimal), "decimal" },
                new object[] { typeof(double), "double" },
                new object[] { typeof(float), "float" },
                new object[] { typeof(int), "int" },
                new object[] { typeof(long), "long" },
                new object[] { typeof(string), "string" }
            };
        }

        [Fact]
        public void ShouldProvideTypeNameForOtherTypes()
        {
            new ValueTypeInfo
            {
                IsNullable = null,
                Type = typeof(Guid)
            }.TypeName.ShouldBe("Guid");
        }

        [Theory, MemberData(nameof(ShouldProvideCorrectTypeNameWithRegardsToNullabilityParams))]
        public void ShouldProvideCorrectTypeNameWithRegardsToNullability(bool? isNullable, Type valueType,
            string expected)
        {
            new ValueTypeInfo
            {
                IsNullable = isNullable,
                Type = valueType
            }.TypeName.ShouldBe(expected);
        }

        public static IEnumerable<object[]> ShouldProvideCorrectTypeNameWithRegardsToNullabilityParams()
        {
            return new[]
            {
                new object[] { null!, typeof(bool), "bool" },
                new object[] { false, typeof(bool), "bool" },
                new object[] { true, typeof(bool), "bool?" }
            };
        }
    }
}