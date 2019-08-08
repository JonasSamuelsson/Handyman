using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal abstract class RequestPipeline<TResponse>
    {
        internal abstract Task<TResponse> Execute(IRequest<TResponse> request, Providers providers, CancellationToken cancellationToken);
    }

    internal class RequestPipeline<TRequest, TResponse> : RequestPipeline<TResponse>
        where TRequest : IRequest<TResponse>
    {
        internal override Task<TResponse> Execute(IRequest<TResponse> request, Providers providers, CancellationToken cancellationToken)
        {
            return Process((TRequest)request, providers, cancellationToken);
        }

        protected Task<TResponse> Process(TRequest request, Providers providers, CancellationToken cancellationToken)
        {
            var filters = providers.RequestFilterProvider.GetFilters<TRequest, TResponse>(providers.ServiceProvider).ToList();

            filters.Sort(CompareFilters);

            var handler = providers.RequestHandlerProvider.GetHandler<TRequest, TResponse>(providers.ServiceProvider);

            var index = 0;
            var length = filters.Count;

            var context = new RequestFilterContext<TRequest>
            {
                CancellationToken = cancellationToken,
                Request = request
            };

            return Execute();

            Task<TResponse> Execute()
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                if (index == length)
                    return handler.Handle(context.Request, context.CancellationToken);

                return filters[index++].Execute(context, Execute);
            }
        }

        private static int CompareFilters(object x, object y)
        {
            return GetSortOrder(x).CompareTo(GetSortOrder(y));
        }

        private static int GetSortOrder(object x)
        {
            return (x as IOrderedFilter)?.Order ?? 0;
        }
    }
}