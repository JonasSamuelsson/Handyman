using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator.Internals
{
    internal static class Extensions
    {
        internal static List<T> ToListOptimized<T>(this IEnumerable<T> enumerable)
        {
            return (enumerable as List<T>) ?? enumerable.ToList();
        }
    }
}
