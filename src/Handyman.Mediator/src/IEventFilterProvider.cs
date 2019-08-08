using System.Collections.Generic;

namespace Handyman.Mediator
{
    public interface IEventFilterProvider
    {
        IEnumerable<IEventFilter<TEvent>> GetFilters<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent;
    }
}