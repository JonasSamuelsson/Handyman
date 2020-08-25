using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal class DefaultEventPipeline<TEvent> : EventPipeline<TEvent>
        where TEvent : IEvent
    {
        internal static readonly EventPipeline<TEvent> DefaultInstance = new DefaultEventPipeline<TEvent>(new WhenAllEventHandlerExecutionStrategy());

        private readonly IEventHandlerExecutionStrategy _executionStrategy;

        internal DefaultEventPipeline(IEventHandlerExecutionStrategy executionStrategy)
        {
            _executionStrategy = executionStrategy;
        }

        protected override Task Execute(List<IEventFilter<TEvent>> filters, List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context)
        {
            return Execute(filters, handlers, _executionStrategy, context);
        }
    }
}