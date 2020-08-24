using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator.Pipelines
{
    internal static class EnumerableExtensions
    {
        internal static List<T> ToListOptimized<T>(this IEnumerable<T> enumerable)
        {
            return (enumerable as List<T>) ?? enumerable.ToList();
        }
    }
}
