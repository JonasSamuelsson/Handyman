using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IMediator
    {
        Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
    }
}