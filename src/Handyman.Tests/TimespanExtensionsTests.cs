using System;
using Shouldly;

namespace Handyman.Tests
{
    public class TimespanExtensionsTests
    {
        public void ShouldGetDateTimeOffsetInThePast()
        {
            var now = DateTimeOffset.Now;
            Configuration.Now = () => now;
            10.Minutes().Ago().ShouldBe(Configuration.Now().Subtract(10.Minutes()));
        }

        public void ShouldGetDateTimeOffsetInTheFuture()
        {
            var now = DateTimeOffset.Now;
            Configuration.Now = () => now;
            10.Minutes().FromNow().ShouldBe(Configuration.Now().Add(10.Minutes()));
        }
    }
}