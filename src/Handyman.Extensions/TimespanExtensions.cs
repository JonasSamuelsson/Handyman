using System;

namespace Handyman.Extensions
{
    public static class TimespanExtensions
    {
        public static DateTimeOffset Ago(this TimeSpan timespan)
        {
            return Configuration.Now().Subtract(timespan);
        }

        public static DateTimeOffset FromNow(this TimeSpan timespan)
        {
            return Configuration.Now().Add(timespan);
        }
    }
}