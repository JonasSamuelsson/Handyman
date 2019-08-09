using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal static class EventPipeline
    {
        internal static Task Execute<TEvent>(Providers providers, TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
        {
            var filters = providers.EventFilterProvider.GetFilters<TEvent>(providers.ServiceProvider).ToListOptimized();
            var handlers = providers.EventHandlerProvider.GetHandlers<TEvent>(providers.ServiceProvider).ToListOptimized();

            return filters.Count != 0
                ? Execute(filters, handlers, @event, cancellationToken)
                : Execute(handlers, @event, cancellationToken);
        }

        private static Task Execute<TEvent>(List<IEventFilter<TEvent>> filters, List<IEventHandler<TEvent>> handlers, TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent
        {
            filters.Sort(FilterComparer.CompareFilters);

            var context = new EventFilterContext<TEvent>
            {
                CancellationToken = cancellationToken,
                Event = @event
            };

            var index = 0;
            var filterCount = filters.Count;

            return Execute();

            Task Execute()
            {
                if (index < filterCount)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();

                    return filters[index++].Execute(context, Execute);
                }

                return EventPipeline.Execute(handlers, @event, cancellationToken);
            }
        }

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
}