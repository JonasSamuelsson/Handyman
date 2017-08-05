using System;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    internal class RequestHandlerAdapter<TRequest> : IRequestHandler<IRequest>
       where TRequest : IRequest
    {
        private readonly IRequestHandler<TRequest> _handler;

        public RequestHandlerAdapter(IRequestHandler<TRequest> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task Handle(IRequest request)
        {
            return _handler.Handle((TRequest)request);
        }
    }

    internal class RequestHandlerAdapter<TRequest, TResponse> : IRequestHandler<IRequest<TResponse>, TResponse>
       where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;

        public RequestHandlerAdapter(IRequestHandler<TRequest, TResponse> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task<TResponse> Handle(IRequest<TResponse> request)
        {
            return _handler.Handle((TRequest)request);
        }
    }
}