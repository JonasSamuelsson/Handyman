using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal abstract class RequestPipeline<TResponse>
    {
        internal abstract Task<TResponse> Execute(IRequest<TResponse> request, IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }

    internal abstract class RequestPipeline<TRequest, TResponse> : RequestPipeline<TResponse>
        where TRequest : IRequest<TResponse>
    {
        internal override Task<TResponse> Execute(IRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var requestContext = new RequestContext<TRequest>
            {
                CancellationToken = cancellationToken,
                Request = (TRequest)request,
                ServiceProvider = serviceProvider
            };

            return Execute(requestContext);
        }

        internal abstract Task<TResponse> Execute(RequestContext<TRequest> requestContext);

        protected Task<TResponse> Execute(List<IRequestFilter<TRequest, TResponse>> filters, IRequestHandler<TRequest, TResponse> handler, IRequestHandlerExecutionStrategy executionStrategy, RequestContext<TRequest> requestContext)
        {
            var filterCount = filters.Count;

            if (filterCount == 0)
            {
                requestContext.CancellationToken.ThrowIfCancellationRequested();
                return executionStrategy.Execute(handler, requestContext);
            }

            // this method call is a no-op if there is only one filter
            filters.Sort(FilterComparer.CompareFilters);

            var filterIndex = 0;

            return ExecuteNextPipelineItem();

            Task<TResponse> ExecuteNextPipelineItem()
            {
                if (filterIndex < filterCount)
                {
                    requestContext.CancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        return filters[filterIndex++].Execute(requestContext, ExecuteNextPipelineItem);
                    }
                    finally
                    {
                        filterIndex--;
                    }
                }

                requestContext.CancellationToken.ThrowIfCancellationRequested();
                return executionStrategy.Execute(handler, requestContext);
            }
        }
    }
}