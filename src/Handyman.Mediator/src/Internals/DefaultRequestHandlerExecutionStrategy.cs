using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Handyman.Mediator.RequestPipelineCustomization;

namespace Handyman.Mediator.Internals
{
    internal class DefaultRequestHandlerExecutionStrategy<TRequest, TResponse> : IRequestHandlerExecutionStrategy<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public static IRequestHandlerExecutionStrategy<TRequest, TResponse> Instance = new DefaultRequestHandlerExecutionStrategy<TRequest, TResponse>();

        public Task<TResponse> Execute(List<IRequestHandler<TRequest, TResponse>> handlers, IRequestPipelineContext<TRequest> context)
        {
            return handlers.Single().Handle(context.Request, context.CancellationToken);
        }
    }
}