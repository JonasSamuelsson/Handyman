using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal class DefaultRequestHandlerExecutionStrategy : IRequestHandlerExecutionStrategy
    {
        public static IRequestHandlerExecutionStrategy Instance = new DefaultRequestHandlerExecutionStrategy();

        public Task<TResponse> Execute<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler,
            RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            return handler.Handle(context.Request, context.CancellationToken);
        }
    }
}