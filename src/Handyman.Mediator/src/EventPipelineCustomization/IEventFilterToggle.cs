using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventFilterToggle
    {
        Task<bool> IsEnabled<TEvent>(Type eventFilterType, EventPipelineContext<TEvent> context)
            where TEvent : IEvent;
    }
}