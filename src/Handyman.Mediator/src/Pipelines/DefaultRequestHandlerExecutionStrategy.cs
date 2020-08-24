using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipelines
{
    internal class DefaultRequestHandlerExecutionStrategy : IRequestHandlerExecutionStrategy
    {
        public static IRequestHandlerExecutionStrategy Instance = new DefaultRequestHandlerExecutionStrategy();

        public Task<TResponse> Execute<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            if (handlers.Count == 0)
            {
                throw new InvalidOperationException($"No handlers for request of type '{context.Request.GetType().FullName}'.");
            }

            if (handlers.Count > 1)
            {
                throw new InvalidOperationException($"Multiple handlers for request of type '{context.Request.GetType().FullName}'.");
            }

            return handlers[0].Handle(context.Request, context.CancellationToken);
        }
    }
}