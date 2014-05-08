using System;
using Shouldly;

namespace Handyman.Tests
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
    }
}