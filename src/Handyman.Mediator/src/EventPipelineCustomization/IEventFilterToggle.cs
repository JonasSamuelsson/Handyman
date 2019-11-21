using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventFilterToggle<TEvent>
        where TEvent : IEvent
    {
        Task<bool> IsEnabled(Type eventFilterType, EventPipelineContext<TEvent> context);
    }
}