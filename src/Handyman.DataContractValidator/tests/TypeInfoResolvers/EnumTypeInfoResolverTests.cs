using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.TypeInfoResolvers;
using Handyman.Extensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Handyman.DataContractValidator.Tests.TypeInfoResolvers
{
    public class EnumTypeInfoResolverTests
    {
        [Theory, MemberData(nameof(ShouldResolveEnumTypeInfosParams))]
        public void ShouldResolveEnumTypeInfos(object o, bool isFlags, bool isNullable, IReadOnlyList<int> ids, IReadOnlyList<string> names)
        {
            var typeInfo = new TypeInfoResolverContext().GetTypeInfo(o).ShouldBeOfType<EnumTypeInfo>();

            typeInfo.IsFlags.ShouldBe(isFlags);
            typeInfo.IsNullable.ShouldBe(isNullable);

            typeInfo.HasIds.ShouldBe(ids != null);
            typeInfo.HasNames.ShouldBe(names != null);

            (typeInfo.HasIds || typeInfo.HasNames).ShouldBeTrue();

            if (!typeInfo.HasIds)
            {
                typeInfo.Names.OrderBy(x => x).ShouldBe(names.OrderBy(x => x));
            }
            else if (!typeInfo.HasNames)
            {
                typeInfo.Ids.OrderBy(x => x).ShouldBe(ids.Select(x => (long)x).OrderBy(x => x));
            }
            else
            {
                if (ids.Count() != names.Count())
                {
                    throw new InvalidOperationException("Invalid test input, number of ids & names don't match.");
                }

                ids.ForEach((x, i) =>
                {
                    typeInfo.Ids.ElementAt(i).ShouldBe(x);
                    typeInfo.Names.ElementAt(i).ShouldBe(names[i]);
                });
            }
        }

        public static IEnumerable<object[]> ShouldResolveEnumTypeInfosParams()
        {
            var flagsIds = new[] { 0, 1, 2, 3 };
            var flagsNames = new[] { "None", "One", "Two", "Both" };
            var flagsValues = flagsIds
                .Select((x, i) => new KeyValuePair<int, string>(x, flagsNames.ElementAt(i)))
                .ToList();

            var valuesIds = new[] { 0, 1 };
            var valuesNames = new[] { "Zero", "One" };
            var valuesValues = valuesIds
                .Select((x, i) => new KeyValuePair<int, string>(x, valuesNames.ElementAt(i)))
                .ToList();

            // object/type, isFlags, isNullable, ids, names

            yield return new object[] { typeof(Flags), true, false, flagsIds, flagsNames };
            yield return new object[] { typeof(Flags?), true, true, flagsIds, flagsNames };
            yield return new object[] { typeof(Values), false, false, valuesIds, valuesNames };
            yield return new object[] { typeof(Values?), false, true, valuesIds, valuesNames };

            yield return new object[]
            {
                new Enum(flagsIds) { Flags = true }, true, false, flagsIds, null
            };
            yield return new object[]
            {
                new Enum(flagsIds) { Flags = true, Nullable = true}, true, true, flagsIds, null
            };
            yield return new object[]
            {
                new Enum(flagsNames) { Flags = true }, true, false, null, flagsNames
            };
            yield return new object[]
            {
                new Enum(flagsNames) { Flags = true, Nullable = true }, true, true, null, flagsNames
            };
            yield return new object[]
            {
                new Enum(flagsValues) { Flags = true }, true, false, flagsIds, flagsNames
            };
            yield return new object[]
            {
                new Enum(flagsValues) { Flags = true, Nullable = true }, true, true, flagsIds, flagsNames
            };

            yield return new object[]
            {
                new Enum(valuesIds), false, false, valuesIds, null
            };
            yield return new object[]
            {
                new Enum(valuesIds) { Nullable = true}, false, true, valuesIds, null
            };
            yield return new object[]
            {
                new Enum(valuesNames), false, false, null, valuesNames
            };
            yield return new object[]
            {
                new Enum(valuesNames) { Nullable = true }, false, true, null, valuesNames
            };
            yield return new object[]
            {
                new Enum(valuesValues), false, false, valuesIds, valuesNames
            };
            yield return new object[]
            {
                new Enum(valuesValues) { Nullable = true }, false, true, valuesIds, valuesNames
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

        private enum Values
        {
            Zero = 0,
            One = 1
        }
    }
}