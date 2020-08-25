using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal class WhenAnyRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _innerHandlers;
        private readonly IServiceProvider _serviceProvider;

        public WhenAnyRequestHandler(IEnumerable<IRequestHandler<TRequest, TResponse>> innerHandlers,
            IServiceProvider serviceProvider)
        {
            _innerHandlers = innerHandlers;
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var tasks = _innerHandlers.Select(x => x.Handle(request, cts.Token)).ToList();
            List<Exception> exceptions = null;

            while (tasks.Count != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var task = await Task.WhenAny(tasks).ConfigureAwait();

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
                    var exceptionHandler = _serviceProvider.GetService<IBackgroundExceptionHandler>();

                    if (exceptionHandler != null)
                    {
                        await exceptionHandler.Handle(exceptions, cancellationToken).ConfigureAwait();
                    }
                }

                cts.Cancel();
                return task.Result;
            }

            throw new AggregateException(exceptions);
        }
    }
}