using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public abstract class VoidRequestHandler<TRequest> : IRequestHandler<TRequest, Void>
        where TRequest : IRequest<Void>
    {
        async Task<Void> IRequestHandler<TRequest, Void>.Handle(TRequest request)
        {
            await Handle(request).ConfigureAwait(false);
            return Void.Instance;
        }

        protected abstract Task Handle(TRequest request);
    }
}