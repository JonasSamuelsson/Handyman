using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public abstract class SynchRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(TRequest request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Handle(request, cancellationToken));
        }

        protected abstract TResponse Handle(TRequest request, CancellationToken cancellationToken);
    }
}