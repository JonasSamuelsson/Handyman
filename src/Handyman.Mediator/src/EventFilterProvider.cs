using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator
{
    internal class EventFilterProvider : IEventFilterProvider
    {
        internal static readonly IEventFilterProvider Instance = new EventFilterProvider();

        public virtual IEnumerable<IEventFilter<TEvent>> GetFilters<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            var type = typeof(IEnumerable<IEventFilter<TEvent>>);

            var filters = (IEnumerable<IEventFilter<TEvent>>)serviceProvider.Invoke(type);

            if (filters is List<IEventFilter<TEvent>> list)
            {
                list.Sort(CompareFilters);
                return list;
            }

            return filters.OrderBy(GetSortOrder);
        }

        private static int CompareFilters(object x, object y)
        {
            return GetSortOrder(x).CompareTo(GetSortOrder(y));
        }

        private static int GetSortOrder(object x)
        {
            return (x as IOrderedPipelineHandler)?.Order ?? 0;
        }
    }
}