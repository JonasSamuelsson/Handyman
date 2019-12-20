using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public static class MediatorExtensions
    {
        public static Task Publish(this IMediator mediator, IEvent @event)
        {
            return mediator.Publish(@event, CancellationToken.None);
        }

        public static Task<TResponse> Send<TResponse>(this IMediator mediator, IRequest<TResponse> request)
        {
            return mediator.Send(request, CancellationToken.None);
        }
    }
}