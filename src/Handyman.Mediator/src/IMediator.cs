using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IMediator : IPublisher
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
    }
}