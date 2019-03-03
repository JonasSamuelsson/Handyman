using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest, Void>
        where TRequest : IRequest<Void>
    {
        async Task<Void> IRequestHandler<TRequest, Void>.Handle(TRequest request, CancellationToken cancellationToken)
        {
            await Handle(request, cancellationToken).ConfigureAwait(false);
            return Void.Instance;
        }

        protected abstract Task Handle(TRequest request, CancellationToken cancellationToken);
    }
}