using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.TypeInfoResolvers;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.TypeInfoResolvers
{
    public class ValueTypeInfoResolverTests
    {
        [Theory]
        [MemberData(nameof(ShouldResolveValueTypeInfoParams))]
        public void ShouldResolveValueTypeInfo(Type type, object ti)
        {
            var expected = (ValueTypeInfo)ti;

            var typeInfo = new TypeInfoResolverContext().GetTypeInfo(type);

            var valueTypeInfo = typeInfo.ShouldBeOfType<ValueTypeInfo>();
            valueTypeInfo.IsNullable.ShouldBe(expected.IsNullable);
            valueTypeInfo.Type.ShouldBe(expected.Type);
        }

        public static IEnumerable<object[]> ShouldResolveValueTypeInfoParams()
        {
            return new[]
            {
                new object[] { typeof(int), new ValueTypeInfo { IsNullable = false, Type = typeof(int) } },
                new object[] { typeof(int?), new ValueTypeInfo { IsNullable = true, Type = typeof(int) } },
                new object[] { typeof(string), new ValueTypeInfo { IsNullable = null, Type = typeof(string) } }
            };
        }
    }
}