using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventFilterToggle
    {
        Task<bool> IsEnabled<TEvent>(EventFilterToggleMetaData toggleMetaData, EventPipelineContext<TEvent> context)
            where TEvent : IEvent;
    }
}