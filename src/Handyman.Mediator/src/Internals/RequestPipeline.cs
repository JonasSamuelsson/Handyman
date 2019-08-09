using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal abstract class RequestPipeline<TResponse>
    {
        internal abstract Task<TResponse> Execute(Providers providers, IRequest<TResponse> request,
            CancellationToken cancellationToken);
    }

    internal class RequestPipeline<TRequest, TResponse> : RequestPipeline<TResponse>
        where TRequest : IRequest<TResponse>
    {
        internal override Task<TResponse> Execute(Providers providers, IRequest<TResponse> request,
            CancellationToken cancellationToken)
        {
            return Process((TRequest)request, providers, cancellationToken);
        }

        protected Task<TResponse> Process(TRequest request, Providers providers, CancellationToken cancellationToken)
        {
            var filters = providers.RequestFilterProvider.GetFilters<TRequest, TResponse>(providers.ServiceProvider).ToListOptimized();
            var handler = providers.RequestHandlerProvider.GetHandler<TRequest, TResponse>(providers.ServiceProvider);

            return filters.Count != 0
                ? Execute(filters, handler, request, cancellationToken)
                : Execute(handler, request, cancellationToken);
        }

        private static Task<TResponse> Execute(List<IRequestFilter<TRequest, TResponse>> filters,
            IRequestHandler<TRequest, TResponse> handler, TRequest request, CancellationToken cancellationToken)
        {
            filters.Sort(FilterComparer.CompareFilters);

            var context = new RequestFilterContext<TRequest>
            {
                CancellationToken = cancellationToken,
                Request = request
            };

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

                return RequestPipeline<TRequest, TResponse>.Execute(handler, request, cancellationToken);
            }
        }

        private static Task<TResponse> Execute(IRequestHandler<TRequest, TResponse> handler, TRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return handler.Handle(request, cancellationToken);
        }
    }
}