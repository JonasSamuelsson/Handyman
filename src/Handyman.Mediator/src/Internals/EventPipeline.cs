using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal static class EventPipeline
    {
        internal static Task Execute<TEvent>(TEvent @event, Providers providers, CancellationToken cancellationToken) where TEvent : IEvent
        {
            var filters = providers.EventFilterProvider.GetFilters<TEvent>(providers.ServiceProvider).ToArray();
            var handlers = providers.EventHandlerProvider.GetHandlers<TEvent>(providers.ServiceProvider).ToArray();

            var index = 0;
            var length = filters.Length;

            var context = new EventFilterContext<TEvent>
            {
                CancellationToken = cancellationToken,
                Event = @event
            };

            return Execute();

            Task Execute()
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                if (index == length)
                    return Task.WhenAll(handlers.Select(x => x.Handle(context.Event, context.CancellationToken)));

                return filters[index++].Execute(context, Execute);
            }
        }
    }
}