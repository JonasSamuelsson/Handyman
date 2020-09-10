using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IRequestDispatcher<TRequest>
        where TRequest : IRequest
    {
        Task Send(TRequest request, CancellationToken cancellationToken);
    }

    public interface IRequestDispatcher<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Send(TRequest request, CancellationToken cancellationToken);
    }
}