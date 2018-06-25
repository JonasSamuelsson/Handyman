using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public abstract class SynchronousVoidResponseRequestHandler<TRequest> : IRequestHandler<TRequest, Void>
        where TRequest : IRequest<Void>
    {
        Task<Void> IRequestHandler<TRequest, Void>.Handle(TRequest request)
        {
            Handle(request);
            return Task.FromResult(Void.Instance);
        }

        protected abstract void Handle(TRequest request);
    }
}