using System.Collections.Generic;

namespace Handyman.Mediator.Pipeline
{
    public class EventPipelineBuilderContext<TEvent>
        where TEvent : IEvent
    {
        public List<IEventFilter<TEvent>> Filters { get; internal set; } = null!;
        public List<IEventHandler<TEvent>> Handlers { get; internal set; } = null!;
        public IEventHandlerExecutionStrategy? HandlerExecutionStrategy { get; set; }
    }
}