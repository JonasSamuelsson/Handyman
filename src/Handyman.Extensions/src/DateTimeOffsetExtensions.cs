using System;

namespace Handyman.Extensions;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset AddWeeks(this DateTimeOffset dateTimeOffset, double weeks)
    {
        return dateTimeOffset.Add(weeks.Weeks());
    }

    public static DateTimeOffset SubtractTicks(this DateTimeOffset dateTimeOffset, long ticks)
    {
        return dateTimeOffset.AddTicks(-ticks);
    }

    public static DateTimeOffset SubtractMilliseconds(this DateTimeOffset dateTimeOffset, double milliseconds)
    {
        return dateTimeOffset.AddMilliseconds(-milliseconds);
    }

    public static DateTimeOffset SubtractSeconds(this DateTimeOffset dateTimeOffset, double seconds)
    {
        return dateTimeOffset.AddSeconds(-seconds);
    }

    public static DateTimeOffset SubtractMinutes(this DateTimeOffset dateTimeOffset, double minutes)
    {
        return dateTimeOffset.AddMinutes(-minutes);
    }

    public static DateTimeOffset SubtractHours(this DateTimeOffset dateTimeOffset, double hours)
    {
        return dateTimeOffset.AddHours(-hours);
    }

    public static DateTimeOffset SubtractDays(this DateTimeOffset dateTimeOffset, double days)
    {
        return dateTimeOffset.AddDays(-days);
    }

    public static DateTimeOffset SubtractWeeks(this DateTimeOffset dateTimeOffset, double weeks)
    {
        return dateTimeOffset.AddWeeks(-weeks);
    }

    public static DateTimeOffset SubtractMonths(this DateTimeOffset dateTimeOffset, int months)
    {
        return dateTimeOffset.AddMonths(-months);
    }

    public static DateTimeOffset SubtractYears(this DateTimeOffset dateTimeOffset, int years)
    {
        return dateTimeOffset.AddYears(-years);
    }

    public static bool IsInThePast(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset < Configuration.Now();
    }

    public static bool IsInTheFuture(this DateTimeOffset dateTimeOffset)
    {
        return Configuration.Now() < dateTimeOffset;
    }
}