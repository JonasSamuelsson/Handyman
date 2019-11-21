using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventHandlerToggle<TEvent>
        where TEvent : IEvent
    {
        Task<bool> IsEnabled(Type eventHandlerType, EventPipelineContext<TEvent> context);
    }
}