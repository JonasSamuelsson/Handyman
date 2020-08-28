using System;
using System.Threading;

namespace Handyman.Mediator.Pipeline
{
    public class EventPipelineContext<TEvent> : IEventFilterContext, IPipelineContext
        where TEvent : IEvent
    {
        public CancellationToken CancellationToken { get; set; }
        public TEvent Event { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        object IPipelineContext.Message => Event;
    }
}