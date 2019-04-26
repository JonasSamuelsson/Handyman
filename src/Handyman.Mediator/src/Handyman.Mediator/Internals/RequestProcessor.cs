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
            var handler = providers.RequestHandlerProvider.GetHandler<TRequest, TResponse>(providers.ServiceProvider);
            var pipelineHandlers = providers.RequestPipelineHandlerProvider.GetHandlers<TRequest, TResponse>(providers.ServiceProvider).ToArray();

            var index = 0;
            var length = pipelineHandlers.Length;

            return Execute(request, cancellationToken);

            Task<TResponse> Execute(TRequest r, CancellationToken ct)
            {
                ct.ThrowIfCancellationRequested();

                if (index == length)
                    return handler.Handle(r, ct);

                var pipelineHandler = pipelineHandlers[index++];
                return pipelineHandler.Handle(r, ct, Execute);
            }
        }
    }
}