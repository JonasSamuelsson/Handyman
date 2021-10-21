using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.TypeInfoResolvers;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.TypeInfoResolvers
{
    public class EnumTypeInfoResolverTests
    {
        [Theory, MemberData(nameof(ShouldResolveEnumTypeInfosParams))]
        public void ShouldResolveEnumTypeInfos(object o, bool isFlags, bool isNullable, Dictionary<long, string> values)
        {
            var typeInfo = new TypeInfoResolverContext().GetTypeInfo(o).ShouldBeOfType<EnumTypeInfo>();

            typeInfo.IsFlags.ShouldBe(isFlags);
            typeInfo.IsNullable.ShouldBe(isNullable);

            typeInfo.Values.Count.ShouldBe(values.Count);

            foreach (var kvp in values)
            {
                typeInfo.Values[kvp.Key].ShouldBe(kvp.Value);
            }
        }

        public static IEnumerable<object[]> ShouldResolveEnumTypeInfosParams()
        {
            var flagsValues = new Dictionary<long, string>
            {
                { 0, "None" },
                { 1, "One" },
                { 2, "Two" },
                { 3, "Both" }
            };

            var regularValues = new Dictionary<long, string>
            {
                { 0, "Zero" },
                { 1, "One" }
            };

            // object/type, isFlags, isNullable, values

            yield return new object[] { typeof(Flags), true, false, flagsValues };
            yield return new object[] { typeof(Flags?), true, true, flagsValues };

            yield return new object[] { typeof(Regular), false, false, regularValues };
            yield return new object[] { typeof(Regular?), false, true, regularValues };

            yield return new object[]
            {
                new Enum { Flags = true, Nullable = false, Values = flagsValues }, true, false, flagsValues
            };
            yield return new object[]
            {
                new Enum { Flags = true, Nullable = true, Values = flagsValues }, true, true, flagsValues
            };

            yield return new object[]
            {
                new Enum { Flags = false, Nullable = false, Values = regularValues}, false, false, regularValues
            };
            yield return new object[]
            {
                new Enum { Flags = false, Nullable = true, Values = regularValues }, false, true, regularValues
            };
        }

        [Flags]
        private enum Flags
        {
            None = 0,
            One = 1,
            Two = 2,
            Both = One | Two
        }

        private enum Regular
        {
            Zero = 0,
            One = 1
        }
    }
}