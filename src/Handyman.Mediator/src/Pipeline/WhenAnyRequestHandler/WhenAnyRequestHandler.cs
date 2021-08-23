using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.WhenAnyRequestHandler
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
            List<Exception>? exceptions = null;

            while (tasks.Count != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var task = await Task.WhenAny(tasks).WithGloballyConfiguredAwait();

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
                    await HandleExceptions(exceptions, cancellationToken).WithGloballyConfiguredAwait();
                }

                cts.Cancel();
                return task.Result;
            }

            throw new AggregateException(exceptions);
        }

        private async Task HandleExceptions(List<Exception> exceptions, CancellationToken cancellationToken)
        {
            var exceptionHandlers = _serviceProvider
                .GetServices<IBackgroundExceptionHandler>()
                .ToListOptimized();

            if (exceptionHandlers.Count == 0)
                return;

            var tasks = exceptionHandlers.Select(x => x.Handle(exceptions, cancellationToken));

            await Task.WhenAll(tasks).WithGloballyConfiguredAwait();
        }
    }
}