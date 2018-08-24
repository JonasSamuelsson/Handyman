using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public static class MediatorExtensions
    {
        public static IEnumerable<Task> Publish<TEvent>(this IMediator mediator, TEvent @event) where TEvent : IEvent
        {
            return mediator.Publish(@event, CancellationToken.None);
        }

        public static Task<TResponse> Send<TResponse>(this IMediator mediator, IRequest<TResponse> request)
        {
            return mediator.Send(request, CancellationToken.None);
        }
    }
}