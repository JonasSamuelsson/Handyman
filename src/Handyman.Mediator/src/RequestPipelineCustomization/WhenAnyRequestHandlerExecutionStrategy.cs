using Handyman.Mediator.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class WhenAnyRequestHandlerExecutionStrategy : IRequestHandlerExecutionStrategy
    {
        public async Task<TResponse> Execute<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            if (handlers.Count <= 1)
            {
                return await DefaultRequestHandlerExecutionStrategy.Instance.Execute(handlers, context).ConfigureAwait(false);
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
            var tasks = handlers.Select(x => x.Handle(context.Request, cts.Token)).ToList();
            List<Exception> exceptions = null;

            while (tasks.Count != 0)
            {
                var task = await Task.WhenAny(tasks).ConfigureAwait(false);

                context.CancellationToken.ThrowIfCancellationRequested();

                if (task.Status != TaskStatus.RanToCompletion)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    exceptions.Add(task.Exception);
                    tasks.Remove(task);
                    continue;
                }

                if (exceptions != null)
                {
                    var exceptionHandler = context.ServiceProvider.GetService<IExceptionHandler>();

                    if (exceptionHandler != null)
                    {
                        await exceptionHandler.Handle(exceptions).ConfigureAwait(false);
                    }
                }

                cts.Cancel();
                return task.Result;
            }

            throw new InvalidOperationException();
        }
    }
}