using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal class CustomizedRequestPipeline<TRequest, TResponse> : RequestPipeline<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        internal List<IRequestFilterSelector> FilterSelectors { get; set; } = null!;
        internal List<IRequestHandlerSelector> HandlerSelectors { get; set; } = null!;
        internal IRequestHandlerExecutionStrategy HandlerExecutionStrategy { get; set; } = null!;

        internal override async Task<TResponse> Execute(RequestContext<TRequest> requestContext)
        {
            var filters = requestContext.ServiceProvider.GetServices<IRequestFilter<TRequest, TResponse>>().ToListOptimized();
            var handlers = requestContext.ServiceProvider.GetServices<IRequestHandler<TRequest, TResponse>>().ToListOptimized();

            foreach (var filterSelector in FilterSelectors)
            {
                requestContext.CancellationToken.ThrowIfCancellationRequested();
                await filterSelector.SelectFilters(filters, requestContext).WithGloballyConfiguredAwait();
            }

            foreach (var handlerSelector in HandlerSelectors)
            {
                requestContext.CancellationToken.ThrowIfCancellationRequested();
                await handlerSelector.SelectHandlers(handlers, requestContext).WithGloballyConfiguredAwait();
            }

            AssertThereIsSingleHandlerToExecute(requestContext, handlers);

            requestContext.CancellationToken.ThrowIfCancellationRequested();

            return await Execute(filters, handlers[0], HandlerExecutionStrategy, requestContext).WithGloballyConfiguredAwait();
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