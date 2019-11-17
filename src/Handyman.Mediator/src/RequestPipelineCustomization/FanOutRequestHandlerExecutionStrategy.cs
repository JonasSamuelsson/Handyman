using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class FanOutRequestHandlerExecutionStrategy<TRequest, TResponse> : IRequestHandlerExecutionStrategy<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Execute(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken))
            {
                var tasks = handlers.Select(x => x.Handle(context.Request, cts.Token)).ToList();

                while (tasks.Count != 0)
                {
                    var task = await Task.WhenAny(tasks).ConfigureAwait(false);

                    if (task.Status != TaskStatus.RanToCompletion)
                    {
                        tasks.Remove(task);
                        continue;
                    }

                    cts.Cancel();
                    return task.Result;
                }

                throw new InvalidOperationException();
            }
        }
    }
}