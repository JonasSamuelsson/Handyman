using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal static class EventProcessor
    {
        internal static Task Process<TEvent>(TEvent @event, Providers providers, CancellationToken cancellationToken) where TEvent : IEvent
        {
            var filters = providers.EventFilterProvider.GetFilters<TEvent>(providers.ServiceProvider).ToArray();
            var handlers = providers.EventHandlerProvider.GetHandlers<TEvent>(providers.ServiceProvider).ToArray();

            var index = 0;
            var length = filters.Length;

            return Execute(@event, cancellationToken);

            Task Execute(TEvent e, CancellationToken ct)
            {
                ct.ThrowIfCancellationRequested();

                if (index == length)
                    return Task.WhenAll(handlers.Select(x => x.Handle(e, ct)));

                return filters[index++].Execute(e, ct, Execute);
            }
        }
    }
}