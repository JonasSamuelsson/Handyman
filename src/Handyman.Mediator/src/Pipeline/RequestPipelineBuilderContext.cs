using System.Collections.Generic;

namespace Handyman.Mediator.Pipeline
{
    public class RequestPipelineBuilderContext<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public List<IRequestFilter<TRequest, TResponse>> Filters { get; internal set; } = null!;
        public List<IRequestHandler<TRequest, TResponse>> Handlers { get; internal set; } = null!;
        public IRequestHandlerExecutionStrategy? HandlerExecutionStrategy { get; set; }
    }
}