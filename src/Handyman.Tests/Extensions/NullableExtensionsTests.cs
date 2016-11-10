using System;
using Handyman.Extensions;
using Shouldly;

namespace Handyman.Tests.Extensions
{
    public class NullableExtensionsTests
    {
        public void ShouldGetValueOrDefault()
        {
            default(IgnoreCase?).GetValueOrDefault(IgnoreCase.Yes).ShouldBe(IgnoreCase.Yes);
            default(IgnoreCase?).GetValueOrDefault(() => IgnoreCase.Yes).ShouldBe(IgnoreCase.Yes);
        }

        public void ShouldGetValueOrThrow()
        {
            Should.Throw<InvalidOperationException>(() => default(IgnoreCase?).GetValueOrThrow());
        }

        public void ShouldCheckIsNull()
        {
            var nullable = default(int?);
            nullable.IsNull().ShouldBe(true);

            nullable = 0;
            nullable.IsNull().ShouldBe(false);
        }

        public void ShouldCheckIsNotNull()
        {
            var nullable = default(int?);
            nullable.IsNotNull().ShouldBe(false);

            nullable = 0;
            nullable.IsNotNull().ShouldBe(true);
        }
    }
}