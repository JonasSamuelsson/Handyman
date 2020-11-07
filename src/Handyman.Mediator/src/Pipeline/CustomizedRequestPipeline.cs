using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal class CustomizedRequestPipeline<TRequest, TResponse> : RequestPipeline<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        internal List<IRequestPipelineBuilder> PipelineBuilders { get; set; } = null!;

        internal override async Task<TResponse> Execute(RequestContext<TRequest> requestContext)
        {
            var serviceProvider = requestContext.ServiceProvider;

            var pipelineBuilderContext = new RequestPipelineBuilderContext<TRequest, TResponse>
            {
                Filters = serviceProvider.GetServices<IRequestFilter<TRequest, TResponse>>().ToListOptimized(),
                HandlerExecutionStrategy = null,
                Handlers = serviceProvider.GetServices<IRequestHandler<TRequest, TResponse>>().ToListOptimized()
            };

            foreach (var pipelineBuilder in PipelineBuilders)
            {
                await pipelineBuilder.Execute(pipelineBuilderContext, requestContext);
            }

            AssertThereIsSingleHandlerToExecute(requestContext, pipelineBuilderContext.Handlers);

            requestContext.CancellationToken.ThrowIfCancellationRequested();

            var filters = pipelineBuilderContext.Filters;
            var handler = pipelineBuilderContext.Handlers[0];
            var handlerExecutionStrategy = pipelineBuilderContext.HandlerExecutionStrategy ?? MediatorDefaults.RequestHandlerExecutionStrategy;

            return await Execute(filters, handler, handlerExecutionStrategy, requestContext).WithGloballyConfiguredAwait();
        }

        private static void AssertThereIsSingleHandlerToExecute(RequestContext<TRequest> requestContext, List<IRequestHandler<TRequest, TResponse>> handlers)
        {
            if (handlers.Count == 0)
            {
                throw new InvalidOperationException($"No handlers for request of type '{requestContext.Request.GetType().FullName}'.");
            }

            if (handlers.Count > 1)
            {
                throw new InvalidOperationException($"Multiple handlers for request of type '{requestContext.Request.GetType().FullName}'.");
            }
        }
    }
}