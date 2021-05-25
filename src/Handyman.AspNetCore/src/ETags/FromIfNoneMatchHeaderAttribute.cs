using System;

namespace Handyman.AspNetCore.ETags
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromIfNoneMatchHeaderAttribute : Attribute
    {
    }
}