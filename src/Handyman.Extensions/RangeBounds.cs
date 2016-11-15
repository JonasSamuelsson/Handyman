using System;

namespace Handyman.Extensions
{
    [Flags]
    public enum RangeBounds
    {
        Exclusive = 0,
        IncludeLower = 1,
        IncludeUpper = 2,
        Inclusive = IncludeLower | IncludeUpper
    }
}