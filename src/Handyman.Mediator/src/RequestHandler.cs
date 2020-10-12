using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.Pipeline;

namespace Handyman.Mediator
{
    public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        async Task<Void> IRequestHandler<TRequest, Void>.Handle(TRequest request, CancellationToken cancellationToken)
        {
            await HandleAsync(request, cancellationToken).WithGloballyConfiguredAwait();
            return Void.Instance;
        }

        protected abstract Task HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}