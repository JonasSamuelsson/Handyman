using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal class DefaultEventPipeline<TEvent> : EventPipeline<TEvent>
        where TEvent : IEvent
    {
        protected override Task Execute(List<IEventFilter<TEvent>> filters, List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context)
        {
            return Execute(filters, handlers, DefaultEventHandlerExecutionStrategy.Instance, context);
        }
    }
}