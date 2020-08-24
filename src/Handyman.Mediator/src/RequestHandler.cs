using System;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.Pipelines;

namespace Handyman.Mediator
{
    public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest, Void>
        where TRequest : IRequest<Void>
    {
        async Task<Void> IRequestHandler<TRequest, Void>.Handle(TRequest request, CancellationToken cancellationToken)
        {
            await HandleAsync(request, cancellationToken).ConfigureAwait();
            return Void.Instance;
        }

        protected virtual Task HandleAsync(TRequest request, CancellationToken cancellationToken)
        {
            Handle(request, cancellationToken);
            return Task.CompletedTask;
        }

        protected virtual void Handle(TRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Please override the HandleAsync or Handle method.");
        }
    }

    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
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