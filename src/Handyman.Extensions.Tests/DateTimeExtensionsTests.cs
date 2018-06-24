using System;
using Shouldly;
using Xunit;

namespace Handyman.Extensions.Tests
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void ShouldAddWeeks()
        {
            var dateTime = DateTime.Now;
            dateTime.AddWeeks(1).ShouldBe(dateTime.Add(1.Weeks()));
        }

        [Fact]
        public void ShouldSubtractTicks()
        {
            var dateTime = DateTime.Now;
            dateTime.SubtractTicks(1).ShouldBe(dateTime.AddTicks(-1));
        }

        [Fact]
        public void ShouldSubtractMilliseconds()
        {
            var dateTime = DateTime.Now;
            dateTime.SubtractMilliseconds(1).ShouldBe(dateTime.AddMilliseconds(-1));
        }

        [Fact]
        public void ShouldSubtractSeconds()
        {
            var dateTime = DateTime.Now;
            dateTime.SubtractSeconds(1).ShouldBe(dateTime.AddSeconds(-1));
        }

        [Fact]
        public void ShouldSubtractMinutes()
        {
            var dateTime = DateTime.Now;
            dateTime.SubtractMinutes(1).ShouldBe(dateTime.AddMinutes(-1));
        }

        [Fact]
        public void ShouldSubtractHours()
        {
            var dateTime = DateTime.Now;
            dateTime.SubtractHours(1).ShouldBe(dateTime.AddHours(-1));
        }

        [Fact]
        public void ShouldSubtractDays()
        {
            var dateTime = DateTime.Now;
            dateTime.SubtractDays(1).ShouldBe(dateTime.AddDays(-1));
        }

        [Fact]
        public void ShouldSubtractWeeks()
        {
            var dateTime = DateTime.Now;
            dateTime.SubtractWeeks(1).ShouldBe(dateTime.AddWeeks(-1));
        }

        [Fact]
        public void ShouldSubtractMonths()
        {
            var dateTime = DateTime.Now;
            dateTime.SubtractMonths(1).ShouldBe(dateTime.AddMonths(-1));
        }

        [Fact]
        public void ShouldSubtractYears()
        {
            var dateTime = DateTime.Now;
            dateTime.SubtractYears(1).ShouldBe(dateTime.AddYears(-1));
        }
    }
}