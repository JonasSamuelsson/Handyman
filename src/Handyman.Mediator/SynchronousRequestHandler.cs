using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public abstract class SynchronousRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(TRequest request)
        {
            return Task.FromResult(Handle(request));
        }

        protected abstract TResponse Handle(TRequest request);
    }
}