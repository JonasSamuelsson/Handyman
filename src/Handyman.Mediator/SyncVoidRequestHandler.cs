using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public abstract class SyncVoidRequestHandler<TRequest> : IRequestHandler<TRequest, Void>
        where TRequest : IRequest<Void>
    {
        Task<Void> IRequestHandler<TRequest, Void>.Handle(TRequest request, CancellationToken cancellationToken)
        {
            Handle(request, cancellationToken);
            return Task.FromResult(Void.Instance);
        }

        protected abstract void Handle(TRequest request, CancellationToken cancellationToken);
    }
}