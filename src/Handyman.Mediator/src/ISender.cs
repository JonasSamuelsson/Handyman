using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface ISender
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
    }

    public interface ISender<TRequest>
        where TRequest : IRequest
    {
        Task Send(TRequest request, CancellationToken cancellationToken);
    }

    public interface ISender<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Send(TRequest request, CancellationToken cancellationToken);
    }
}