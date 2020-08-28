using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public interface IEventFilterSelector
    {
        Task SelectFilters<TEvent>(List<IEventFilter<TEvent>> filters, EventPipelineContext<TEvent> context)
            where TEvent : IEvent;
    }
}