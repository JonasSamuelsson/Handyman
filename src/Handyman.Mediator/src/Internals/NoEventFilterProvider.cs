using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator.Internals
{
    internal class NoEventFilterProvider : IEventFilterProvider
    {
        internal static IEventFilterProvider Instance = new NoEventFilterProvider();

        public IEnumerable<IEventFilter<TEvent>> GetFilters<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            return Enumerable.Empty<IEventFilter<TEvent>>();
        }
    }
}