using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal class CustomizedRequestPipeline<TRequest, TResponse> : RequestPipeline<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        internal List<IRequestFilterSelector> FilterSelectors { get; set; }
        internal List<IRequestHandlerSelector> HandlerSelectors { get; set; }
        internal IRequestHandlerExecutionStrategy HandlerExecutionStrategy { get; set; }

        internal override async Task<TResponse> Execute(RequestPipelineContext<TRequest> context)
        {
            var filters = context.ServiceProvider.GetServices<IRequestFilter<TRequest, TResponse>>().ToListOptimized();
            var handlers = context.ServiceProvider.GetServices<IRequestHandler<TRequest, TResponse>>().ToListOptimized();

            foreach (var filterSelector in FilterSelectors)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                await filterSelector.SelectFilters(filters, context).ConfigureAwait();
            }

            foreach (var handlerSelector in HandlerSelectors)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                await handlerSelector.SelectHandlers(handlers, context).ConfigureAwait();
            }

            AssertThereIsSingleHandlerToExecute(context, handlers);

            context.CancellationToken.ThrowIfCancellationRequested();

            return await Execute(filters, handlers[0], HandlerExecutionStrategy, context).ConfigureAwait();
        }

        private static void AssertThereIsSingleHandlerToExecute(RequestPipelineContext<TRequest> context, List<IRequestHandler<TRequest, TResponse>> handlers)
        {
            if (handlers.Count == 0)
            {
                throw new InvalidOperationException($"No handlers for request of type '{context.Request.GetType().FullName}'.");
            }

            if (handlers.Count > 1)
            {
                throw new InvalidOperationException($"Multiple handlers for request of type '{context.Request.GetType().FullName}'.");
            }
        }
    }
}