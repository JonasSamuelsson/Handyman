using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal class RequestHandler<TRequest, TResponse> : IRequestHandler<IRequest<TResponse>, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;

        public RequestHandler(IRequestHandler<TRequest, TResponse> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            return Execute((TRequest)request, cancellationToken);
        }

        protected virtual Task<TResponse> Execute(TRequest request, CancellationToken cancellationToken)
        {
            return _handler.Handle(request, cancellationToken);
        }
    }
}