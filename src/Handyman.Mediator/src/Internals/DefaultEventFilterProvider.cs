using System.Collections.Generic;

namespace Handyman.Mediator.Internals
{
    internal class DefaultEventFilterProvider : IEventFilterProvider
    {
        internal static readonly IEventFilterProvider Instance = new DefaultEventFilterProvider();

        private DefaultEventFilterProvider() { }

        public virtual IEnumerable<IEventFilter<TEvent>> GetFilters<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
        {
            return (IEnumerable<IEventFilter<TEvent>>)serviceProvider.Invoke(typeof(IEnumerable<IEventFilter<TEvent>>));
        }
    }
}