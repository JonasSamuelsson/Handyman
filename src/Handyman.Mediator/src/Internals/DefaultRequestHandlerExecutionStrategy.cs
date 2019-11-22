using Handyman.Mediator.RequestPipelineCustomization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal class DefaultRequestHandlerExecutionStrategy<TRequest, TResponse> : IRequestHandlerExecutionStrategy<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public static IRequestHandlerExecutionStrategy<TRequest, TResponse> Instance = new DefaultRequestHandlerExecutionStrategy<TRequest, TResponse>();

        public Task<TResponse> Execute(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
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