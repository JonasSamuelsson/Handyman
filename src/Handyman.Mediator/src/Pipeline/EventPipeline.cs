using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal abstract class EventPipeline
    {
        internal abstract Task Execute(IEvent @event, IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }

    internal abstract class EventPipeline<TEvent> : EventPipeline
        where TEvent : IEvent
    {
        internal override Task Execute(IEvent @event, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var filters = serviceProvider.GetServices<IEventFilter<TEvent>>().ToListOptimized();
            var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>().ToListOptimized();

            var eventContext = new EventContext<TEvent>
            {
                CancellationToken = cancellationToken,
                Event = (TEvent)@event,
                ServiceProvider = serviceProvider
            };

            return Execute(filters, handlers, eventContext);
        }

        protected abstract Task Execute(List<IEventFilter<TEvent>> filters, List<IEventHandler<TEvent>> handlers, EventContext<TEvent> eventContext);

        protected Task Execute(List<IEventFilter<TEvent>> filters, List<IEventHandler<TEvent>> handlers, IEventHandlerExecutionStrategy executionStrategy, EventContext<TEvent> eventContext)
        {
            var filterCount = filters.Count;

            if (filterCount == 0)
            {
                eventContext.CancellationToken.ThrowIfCancellationRequested();
                return executionStrategy.Execute(handlers, eventContext);
            }

            filters.Sort(FilterComparer.CompareFilters);

            var index = 0;

            return Execute();

            Task Execute()
            {
                if (index < filterCount)
                {
                    eventContext.CancellationToken.ThrowIfCancellationRequested();
                    return filters[index++].Execute(eventContext, Execute);
                }

                eventContext.CancellationToken.ThrowIfCancellationRequested();
                return executionStrategy.Execute(handlers, eventContext);
            }
        }
    }
}