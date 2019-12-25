using Handyman.Mediator.RequestPipelineCustomization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
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
                await filterSelector.SelectFilters(filters, context).ConfigureAwait(false);
            }

            foreach (var handlerSelector in HandlerSelectors)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                await handlerSelector.SelectHandlers(handlers, context).ConfigureAwait(false);
            }

            context.CancellationToken.ThrowIfCancellationRequested();
            return await Execute(filters, ctx => HandlerExecutionStrategy.Execute(handlers, ctx), context).ConfigureAwait(false);
        }
    }
}