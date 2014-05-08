using System;

namespace Handyman
{
    public static class Configuration
    {
        public static Func<DateTimeOffset> Now = () => System.DateTimeOffset.Now;
    }
}