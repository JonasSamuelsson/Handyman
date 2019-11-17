using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Handyman.Mediator.EventPipelineCustomization;

namespace Handyman.Mediator.Internals
{
    internal class DefaultEventHandlerExecutionStrategy<TEvent> : IEventHandlerExecutionStrategy<TEvent>
        where TEvent : IEvent
    {
        public static readonly IEventHandlerExecutionStrategy<TEvent> Instance = new DefaultEventHandlerExecutionStrategy<TEvent>();

        public Task Execute(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context)
        {
            return Task.WhenAll(handlers.Select(x => x.Handle(context.Event, context.CancellationToken)));
        }
    }
}