using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public sealed class WhenAllEventHandlerExecutionStrategy : IEventHandlerExecutionStrategy
    {
        public static readonly IEventHandlerExecutionStrategy Instance = new WhenAllEventHandlerExecutionStrategy();

        public Task Execute<TEvent>(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            return Task.WhenAll(handlers.Select(x => x.Handle(context.Event, context.CancellationToken)));
        }
    }
}