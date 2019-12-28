using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventHandlerToggle
    {
        Task<bool> IsEnabled<TEvent>(Type eventHandlerType, EventPipelineContext<TEvent> context)
            where TEvent : IEvent;
    }
}