using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    internal class PipelinedRequestHandler<TRequest, TResponse> : RequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private int _index;
        private readonly IReadOnlyList<IRequestPipelineHandler<TRequest, TResponse>> _pipelineHandlers;

        public PipelinedRequestHandler(IEnumerable<IRequestPipelineHandler<TRequest, TResponse>> pipelineHandlers, IRequestHandler<TRequest, TResponse> handler)
            : base(handler)
        {
            _pipelineHandlers = (pipelineHandlers as IReadOnlyList<IRequestPipelineHandler<TRequest, TResponse>> ?? pipelineHandlers?.ToArray())
                                ?? throw new ArgumentNullException(nameof(pipelineHandlers));
        }

        protected override Task<TResponse> Execute(TRequest request, CancellationToken cancellationToken)
        {
            var index = _index++;

            if (index == _pipelineHandlers.Count)
                return base.Execute(request, cancellationToken);

            var pipelineHandler = _pipelineHandlers[index];
            return pipelineHandler.Execute(request, cancellationToken, Execute);
        }
    }
}