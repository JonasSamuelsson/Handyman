using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal class DefaultEventPipeline<TEvent> : EventPipeline<TEvent>
        where TEvent : IEvent
    {
        internal static readonly EventPipeline<TEvent> Instance = new DefaultEventPipeline<TEvent>();

        private DefaultEventPipeline() { }

        protected override Task Execute(List<IEventFilter<TEvent>> filters, List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context)
        {
            return Execute(filters, handlers, DefaultEventHandlerExecutionStrategy.Instance, context);
        }
    }
}