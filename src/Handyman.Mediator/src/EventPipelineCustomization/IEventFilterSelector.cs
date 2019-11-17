using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventFilterSelector<TEvent> where TEvent : IEvent
    {
        Task SelectFilters(List<IEventFilter<TEvent>> filters, EventPipelineContext<TEvent> context);
    }
}