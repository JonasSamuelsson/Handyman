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
            var context = new RequestPipelineContext<TRequest>
            {
                CancellationToken = cancellationToken,
                Request = (TRequest)request,
                ServiceProvider = serviceProvider
            };

            return Execute(context);
        }

        internal abstract Task<TResponse> Execute(RequestPipelineContext<TRequest> context);

        protected Task<TResponse> Execute(List<IRequestFilter<TRequest, TResponse>> filters,
            RequestHandlerDelegate<TRequest, TResponse> handler, RequestPipelineContext<TRequest> context)
        {
            if (filters.Count == 0)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                return handler.Invoke(context);
            }

            filters.Sort(FilterComparer.CompareFilters);

            var index = 0;
            var filterCount = filters.Count;


            return Execute();

            Task<TResponse> Execute()
            {
                if (index < filterCount)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    return filters[index++].Execute(context, Execute);
                }

                context.CancellationToken.ThrowIfCancellationRequested();
                return handler.Invoke(context);
            }
        }
    }
}