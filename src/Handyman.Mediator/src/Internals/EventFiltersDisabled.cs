using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator.Internals
{
    public class EventFiltersDisabled : IEventFilterProvider
    {
        public static IEventFilterProvider Instance = new EventFiltersDisabled();

        private EventFiltersDisabled() { }

        public IEnumerable<IEventFilter<TEvent>> GetFilters<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            return Enumerable.Empty<IEventFilter<TEvent>>();
        }
    }
}