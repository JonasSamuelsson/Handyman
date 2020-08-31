using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public interface IEventFilterSelector
    {
        Task SelectFilters<TEvent>(List<IEventFilter<TEvent>> filters, EventContext<TEvent> eventContext)
            where TEvent : IEvent;
    }
}