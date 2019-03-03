using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal static class EventProcessor
    {
        internal static Task Process<TEvent>(TEvent @event, Providers providers, CancellationToken cancellationToken) where TEvent : IEvent
        {
            var pipelineHandlers = providers.EventPipelineHandlerProvider.GetHandlers<TEvent>(providers.ServiceProvider).ToArray();
            var handlers = providers.EventHandlerProvider.GetHandlers<TEvent>(providers.ServiceProvider).ToArray();

            var index = 0;
            var length = pipelineHandlers.Length;

            return Execute(@event, cancellationToken);

            Task Execute(TEvent e, CancellationToken ct)
            {
                if (index == length)
                    return Task.WhenAll(handlers.Select(x => x.Handle(e, ct)));

                var pipelineHandler = pipelineHandlers[index++];
                return pipelineHandler.Handle(e, ct, Execute);
            }
        }
    }
}