using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class FanOutRequestHandlerExecutionStrategy<TRequest, TResponse> : IRequestHandlerExecutionStrategy<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Execute(List<IRequestHandler<TRequest, TResponse>> handlers, IRequestPipelineContext<TRequest> context)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken))
            {
                var tasks = handlers.Select(x => x.Handle(context.Request, cts.Token)).ToList();
                var task = await Task.WhenAny(tasks).ConfigureAwait(false);
                var response = await task.ConfigureAwait(false);
                cts.Cancel();
                return response;
            }
        }
    }
}