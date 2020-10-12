using Handyman.Mediator.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        async Task<Void> IRequestHandler<TRequest, Void>.Handle(TRequest request, CancellationToken cancellationToken)
        {
            await Handle(request, cancellationToken).WithGloballyConfiguredAwait();
            return Void.Instance;
        }

        protected abstract Task Handle(TRequest request, CancellationToken cancellationToken);
    }
}