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
            var pipelineBuilderContext = CreatePipelineBuilderContext(requestContext);

            await ExecutePipelineBuilders(pipelineBuilderContext, requestContext);

            AssertThereIsSingleHandlerToExecute(pipelineBuilderContext.Handlers);

            return await ExecutePipeline(pipelineBuilderContext, requestContext);
        }

        private static RequestPipelineBuilderContext<TRequest, TResponse> CreatePipelineBuilderContext(RequestContext<TRequest> requestContext)
        {
            var serviceProvider = requestContext.ServiceProvider;

            return new RequestPipelineBuilderContext<TRequest, TResponse>
            {
                Filters = serviceProvider.GetServices<IRequestFilter<TRequest, TResponse>>().ToListOptimized(),
                HandlerExecutionStrategy = null,
                Handlers = serviceProvider.GetServices<IRequestHandler<TRequest, TResponse>>().ToListOptimized()
            };
        }

        private async Task ExecutePipelineBuilders(RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext, RequestContext<TRequest> requestContext)
        {
            foreach (var pipelineBuilder in PipelineBuilders)
            {
                await pipelineBuilder.Execute(pipelineBuilderContext, requestContext);
            }
        }

        private static void AssertThereIsSingleHandlerToExecute(List<IRequestHandler<TRequest, TResponse>> handlers)
        {
            if (handlers.Count == 0)
            {
                throw new InvalidOperationException($"No handlers for request of type '{typeof(TRequest).FullName}'.");
            }

            if (handlers.Count > 1)
            {
                throw new InvalidOperationException($"Multiple handlers for request of type '{typeof(TRequest).FullName}'.");
            }
        }

        private async Task<TResponse> ExecutePipeline(
            RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext,
            RequestContext<TRequest> requestContext)
        {
            requestContext.CancellationToken.ThrowIfCancellationRequested();

            var filters = pipelineBuilderContext.Filters;
            var handler = pipelineBuilderContext.Handlers[0];
            var handlerExecutionStrategy = pipelineBuilderContext.HandlerExecutionStrategy ??
                                           MediatorDefaults.RequestHandlerExecutionStrategy;

            return await Execute(filters, handler, handlerExecutionStrategy, requestContext).WithGloballyConfiguredAwait();
        }
    }
}