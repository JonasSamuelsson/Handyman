using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestHandlerExperiment
{
    internal class RequestHandlerExperimentHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _baselineHandler;
        private readonly List<IRequestHandler<TRequest, TResponse>> _experimentalHandlers;
        private readonly List<IRequestHandlerExperimentObserver> _observers;

        public RequestHandlerExperimentHandler(IRequestHandler<TRequest, TResponse> baselineHandler, List<IRequestHandler<TRequest, TResponse>> experimentalHandlers, List<IRequestHandlerExperimentObserver> observers)
        {
            _baselineHandler = baselineHandler;
            _experimentalHandlers = experimentalHandlers;
            _observers = observers;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<RequestHandlerExperimentExecution<TRequest, TResponse>>> { Execute(_baselineHandler, request, cancellationToken) };

            foreach (var experimentalHandler in _experimentalHandlers)
            {
                tasks.Add(Execute(experimentalHandler, request, cancellationToken));
            }

            var executions = await Task.WhenAll(tasks).WithGloballyConfiguredAwait();
            var baselineExecution = executions.Single(x => x.Handler == _baselineHandler);
            var experimentalExecutions = executions.Where(x => x != baselineExecution).ToList();

            var experiment = new RequestHandlerExperiment<TRequest, TResponse>
            {
                BaselineExecution = baselineExecution,
                CancellationToken = cancellationToken,
                ExperimentalExecutions = experimentalExecutions,
                Request = request
            };

            if (_observers.Count != 0)
            {
                var observerTasks = _observers.Select(x => x.Observe(experiment));

                await Task.WhenAll(observerTasks).WithGloballyConfiguredAwait();
            }

            return await baselineExecution.Task.WithGloballyConfiguredAwait();
        }

        private static async Task<RequestHandlerExperimentExecution<TRequest, TResponse>> Execute(IRequestHandler<TRequest, TResponse> handler, TRequest request, CancellationToken cancellationToken)
        {
            // if the the code in handler.Handle(...) throws before doing any async/await the task will not be returned, hence the outer try/catch

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var task = handler.Handle(request, cancellationToken);

                try
                {
                    await task.WithGloballyConfiguredAwait();
                }
                catch
                {
                    ; // intentionally empty
                }

                return new RequestHandlerExperimentExecution<TRequest, TResponse>
                {
                    Duration = stopwatch.Elapsed,
                    Handler = handler,
                    Task = task
                };
            }
            catch (Exception exception)
            {
                return new RequestHandlerExperimentExecution<TRequest, TResponse>
                {
                    Duration = stopwatch.Elapsed,
                    Handler = handler,
                    Task = Task.FromException<TResponse>(exception)
                };
            }
        }
    }
}