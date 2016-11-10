using System;

namespace Handyman.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime AddWeeks(this DateTime dateTime, double weeks)
        {
            return dateTime.Add(weeks.Weeks());
        }

        public static DateTime SubtractTicks(this DateTime dateTime, long ticks)
        {
            return dateTime.AddTicks(-ticks);
        }

        public static DateTime SubtractMilliseconds(this DateTime dateTime, double milliseconds)
        {
            return dateTime.AddMilliseconds(-milliseconds);
        }

        public static DateTime SubtractSeconds(this DateTime dateTime, double seconds)
        {
            return dateTime.AddSeconds(-seconds);
        }

        public static DateTime SubtractMinutes(this DateTime dateTime, double minutes)
        {
            return dateTime.AddMinutes(-minutes);
        }

        public static DateTime SubtractHours(this DateTime dateTime, double hours)
        {
            return dateTime.AddHours(-hours);
        }

        public static DateTime SubtractDays(this DateTime dateTime, double days)
        {
            return dateTime.AddDays(-days);
        }

        public static DateTime SubtractMonths(this DateTime dateTime, int months)
        {
            return dateTime.AddMonths(-months);
        }

        public static DateTime SubtractWeeks(this DateTime dateTime, int weeks)
        {
            return dateTime.AddWeeks(-weeks);
        }

        public static DateTime SubtractYears(this DateTime dateTime, int years)
        {
            return dateTime.AddYears(-years);
        }
    }
}