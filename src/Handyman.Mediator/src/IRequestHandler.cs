using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public interface IRequestHandler<TRequest> : IRequestHandler<TRequest, Void>
        where TRequest : IRequest
    {
    }
}