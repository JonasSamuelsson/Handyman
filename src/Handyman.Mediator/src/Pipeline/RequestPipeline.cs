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
            if (filters.Count == 0)
            {
                requestContext.CancellationToken.ThrowIfCancellationRequested();
                return executionStrategy.Execute(handler, requestContext);
            }

            filters.Sort(FilterComparer.CompareFilters);

            var index = 0;
            var filterCount = filters.Count;

            return Execute();

            Task<TResponse> Execute()
            {
                if (index < filterCount)
                {
                    requestContext.CancellationToken.ThrowIfCancellationRequested();
                    return filters[index++].Execute(requestContext, Execute);
                }

                requestContext.CancellationToken.ThrowIfCancellationRequested();
                return executionStrategy.Execute(handler, requestContext);
            }
        }
    }
}