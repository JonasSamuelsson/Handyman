using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IMediator
    {
        Task Publish(IEvent @event, CancellationToken cancellationToken);
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
    }
}