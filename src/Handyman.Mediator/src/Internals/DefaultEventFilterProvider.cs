using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.Internals
{
    internal class DefaultEventFilterProvider : IEventFilterProvider
    {
        internal static readonly IEventFilterProvider Instance = new DefaultEventFilterProvider();

        private DefaultEventFilterProvider() { }

        public virtual IEnumerable<IEventFilter<TEvent>> GetFilters<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            var attribute = typeof(TEvent).GetCustomAttributes<EventFilterProviderAttribute>(true).SingleOrDefault();

            if (attribute == null)
                return GetDefaultFilters<TEvent>(serviceProvider);

            var filters = GetDefaultFilters<TEvent>(serviceProvider).ToListOptimized();
            filters.AddRange(attribute.GetFilters<TEvent>(serviceProvider));
            return filters;
        }

        private static IEnumerable<IEventFilter<TEvent>> GetDefaultFilters<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            return (IEnumerable<IEventFilter<TEvent>>)serviceProvider.Invoke(typeof(IEnumerable<IEventFilter<TEvent>>));
        }
    }
}