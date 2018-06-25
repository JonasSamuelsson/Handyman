using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    internal class DelegatingRequestHandler<TRequest, TResponse> : IRequestHandler<IRequest<TResponse>, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private int _index;
        private readonly IReadOnlyList<IRequestPipelineHandler<TRequest, TResponse>> _pipelineHandlers;
        private readonly IRequestHandler<TRequest, TResponse> _handler;

        public DelegatingRequestHandler(IEnumerable<IRequestPipelineHandler<TRequest, TResponse>> pipelineHandlers, IRequestHandler<TRequest, TResponse> handler)
        {
            _pipelineHandlers = (pipelineHandlers as IReadOnlyList<IRequestPipelineHandler<TRequest, TResponse>> ?? pipelineHandlers?.ToArray()) ?? throw new ArgumentNullException(nameof(pipelineHandlers));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task<TResponse> Handle(IRequest<TResponse> request)
        {
            return Execute((TRequest)request);
        }

        private Task<TResponse> Execute(TRequest request)
        {
            var index = _index++;

            if (index == _pipelineHandlers.Count)
                return _handler.Handle(request);

            var pipelineHandler = _pipelineHandlers[index];
            return pipelineHandler.Execute(request, Execute);
        }
    }
}