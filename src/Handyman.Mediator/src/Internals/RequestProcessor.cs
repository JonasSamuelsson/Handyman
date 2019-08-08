using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal abstract class RequestProcessor<TResponse>
    {
        internal abstract Task<TResponse> Process(IRequest<TResponse> request, Providers providers, CancellationToken cancellationToken);
    }

    internal class RequestProcessor<TRequest, TResponse> : RequestProcessor<TResponse>
        where TRequest : IRequest<TResponse>
    {
        internal override Task<TResponse> Process(IRequest<TResponse> request, Providers providers, CancellationToken cancellationToken)
        {
            return Process((TRequest)request, providers, cancellationToken);
        }

        protected Task<TResponse> Process(TRequest request, Providers providers, CancellationToken cancellationToken)
        {
            var filters = providers.RequestFilterProvider.GetFilters<TRequest, TResponse>(providers.ServiceProvider).ToArray();
            var handler = providers.RequestHandlerProvider.GetHandler<TRequest, TResponse>(providers.ServiceProvider);

            var index = 0;
            var length = filters.Length;

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
    }
}