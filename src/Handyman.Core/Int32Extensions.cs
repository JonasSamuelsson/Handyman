using System;

namespace Handyman
{
    public static class Int32Extensions
    {
        public static TimeSpan Ticks(this int ticks)
        {
            return TimeSpan.FromTicks(ticks);
        }

        public static TimeSpan Milliseconds(this int milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }

        public static TimeSpan Seconds(this int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        public static TimeSpan Minutes(this int minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }

        public static TimeSpan Hours(this int hours)
        {
            return TimeSpan.FromHours(hours);
        }

        public static TimeSpan Days(this int days)
        {
            return TimeSpan.FromDays(days);
        }

        public static TimeSpan Weeks(this int weeks)
        {
            return TimeSpan.FromDays(weeks * 7);
        }
    }
}