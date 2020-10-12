using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public abstract class SyncRequestHandler<TRequest> : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        Task<Void> IRequestHandler<TRequest, Void>.Handle(TRequest request, CancellationToken cancellationToken)
        {
            Handle(request, cancellationToken);
            return Task.FromResult(Void.Instance);
        }

        protected abstract void Handle(TRequest request, CancellationToken cancellationToken);
    }

    public abstract class SyncRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
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