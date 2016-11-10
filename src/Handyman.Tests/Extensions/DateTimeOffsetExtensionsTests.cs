using System;
using Handyman.Extensions;
using Shouldly;

namespace Handyman.Tests.Extensions
{
    public class DateTimeOffsetExtensionsTests
    {
        public void ShouldAddWeeks()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            dateTimeOffset.AddWeeks(1).ShouldBe(dateTimeOffset.Add(1.Weeks()));
        }

        public void ShouldSubtractTicks()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            dateTimeOffset.SubtractTicks(1).ShouldBe(dateTimeOffset.AddTicks(-1));
        }

        public void ShouldSubtractMilliseconds()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            dateTimeOffset.SubtractMilliseconds(1).ShouldBe(dateTimeOffset.AddMilliseconds(-1));
        }

        public void ShouldSubtractSeconds()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            dateTimeOffset.SubtractSeconds(1).ShouldBe(dateTimeOffset.AddSeconds(-1));
        }

        public void ShouldSubtractMinutes()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            dateTimeOffset.SubtractMinutes(1).ShouldBe(dateTimeOffset.AddMinutes(-1));
        }

        public void ShouldSubtractHours()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            dateTimeOffset.SubtractHours(1).ShouldBe(dateTimeOffset.AddHours(-1));
        }

        public void ShouldSubtractDays()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            dateTimeOffset.SubtractDays(1).ShouldBe(dateTimeOffset.AddDays(-1));
        }

        public void ShouldSubtractWeeks()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            dateTimeOffset.SubtractWeeks(1).ShouldBe(dateTimeOffset.AddWeeks(-1));
        }

        public void ShouldSubtractMonths()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            dateTimeOffset.SubtractMonths(1).ShouldBe(dateTimeOffset.AddMonths(-1));
        }

        public void ShouldSubtractYears()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            dateTimeOffset.SubtractYears(1).ShouldBe(dateTimeOffset.AddYears(-1));
        }

        public void ShouldCheckIfDateTimeOffsetIdInThePast()
        {
            DateTimeOffset.Now.Add(1.Hours()).IsInThePast().ShouldBe(false);
            DateTimeOffset.Now.Subtract(1.Hours()).IsInThePast().ShouldBe(true);
        }

        public void ShouldCheckIfDateTimeOffsetIdInTheFuture()
        {
            DateTimeOffset.Now.Add(1.Hours()).IsInTheFuture().ShouldBe(true);
            DateTimeOffset.Now.Subtract(1.Hours()).IsInTheFuture().ShouldBe(false);
        }
    }
}