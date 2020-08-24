using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipelines
{
    internal abstract class EventPipeline
    {
        internal abstract Task Execute(IEvent @event, IServiceProvider serviceProvider, CancellationToken cancellationToken);

        private static Task Execute<TEvent>(List<IEventHandler<TEvent>> handlers, TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
        {
            cancellationToken.ThrowIfCancellationRequested();

            var tasks = new List<Task>(handlers.Count);

            foreach (var handler in handlers)
            {
                tasks.Add(handler.Handle(@event, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }
    }

    internal abstract class EventPipeline<TEvent> : EventPipeline
        where TEvent : IEvent
    {
        internal override Task Execute(IEvent @event, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var filters = serviceProvider.GetServices<IEventFilter<TEvent>>().ToListOptimized();
            var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>().ToListOptimized();

            var context = new EventPipelineContext<TEvent>
            {
                CancellationToken = cancellationToken,
                Event = (TEvent)@event,
                ServiceProvider = serviceProvider
            };

            return Execute(filters, handlers, context);
        }

        protected abstract Task Execute(List<IEventFilter<TEvent>> filters, List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context);

        protected Task Execute(List<IEventFilter<TEvent>> filters, List<IEventHandler<TEvent>> handlers, IEventHandlerExecutionStrategy executionStrategy, EventPipelineContext<TEvent> context)
        {
            var filterCount = filters.Count;

            if (filterCount == 0)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                return executionStrategy.Execute(handlers, context);
            }

            filters.Sort(FilterComparer.CompareFilters);

            var index = 0;

            return Execute();

            Task Execute()
            {
                if (index < filterCount)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    return filters[index++].Execute(context, Execute);
                }

                context.CancellationToken.ThrowIfCancellationRequested();
                return executionStrategy.Execute(handlers, context);
            }
        }
    }
}