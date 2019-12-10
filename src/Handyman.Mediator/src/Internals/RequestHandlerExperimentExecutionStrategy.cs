using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.RequestPipelineCustomization;

namespace Handyman.Mediator.Internals
{
    internal class RequestHandlerExperimentExecutionStrategy<TRequest, TResponse> : IRequestHandlerExecutionStrategy<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Type _baselineHandlerType;

        public RequestHandlerExperimentExecutionStrategy(Type baselineHandlerType)
        {
            _baselineHandlerType = baselineHandlerType;
        }

        public async Task<TResponse> Execute(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
        {
            if (handlers.Count <= 1)
            {
                return await DefaultRequestHandlerExecutionStrategy<TRequest, TResponse>.Instance.Execute(handlers, context).ConfigureAwait(false);
            }

            var baselineHandler = GetBaselineHandler(handlers);

            var toggle = context.ServiceProvider.GetRequiredService<IRequestHandlerExperimentToggle<TRequest, TResponse>>();

            var request = context.Request;
            var cancellationToken = context.CancellationToken;

            if (!await toggle.IsEnabled(request, cancellationToken).ConfigureAwait(false))
            {
                return await baselineHandler.Handle(request, cancellationToken).ConfigureAwait(false);
            }

            var observer = context.ServiceProvider.GetRequiredService<IRequestHandlerExperimentObserver<TRequest, TResponse>>();
            var tasks = handlers.Select(handler => Execute(handler, request, cancellationToken)).ToListOptimized();
            var executions = await Task.WhenAll(tasks).ConfigureAwait(false);
            var baselineExecution = executions.Single(x => x.Handler == baselineHandler);
            var experimentExecutions = executions.Where(x => x != baselineExecution).ToList();

            var experiment = new RequestHandlerExperiment<TRequest, TResponse>
            {
                BaselineExecution = baselineExecution,
                ExperimentalExecutions = experimentExecutions,
                Request = request
            };

            await observer.Observe(experiment).ConfigureAwait(false);

            return await baselineExecution.Task.ConfigureAwait(false);
        }

        private IRequestHandler<TRequest, TResponse> GetBaselineHandler(List<IRequestHandler<TRequest, TResponse>> handlers)
        {
            IRequestHandler<TRequest, TResponse> baselineHandler = null;

            foreach (var handler in handlers)
            {
                if (handler.GetType() != _baselineHandlerType)
                    continue;

                if (baselineHandler != null)
                    throw new InvalidOperationException($"{typeof(TRequest).FullName} experiment has multiple baseline handlers.");

                baselineHandler = handler;
            }

            if (baselineHandler == null)
                throw new InvalidOperationException($"{typeof(TRequest).FullName} experiment does not have a baseline handler.");

            return baselineHandler;
        }

        private static Task<RequestHandlerExperimentExecution<TRequest, TResponse>> Execute(IRequestHandler<TRequest, TResponse> handler, TRequest request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            return handler.Handle(request, cancellationToken)
                .ContinueWith(task =>
                {
                    var duration = stopwatch.Elapsed;
                    return new RequestHandlerExperimentExecution<TRequest, TResponse>
                    {
                        Duration = duration,
                        Handler = handler,
                        Task = task
                    };
                }, TaskScheduler.Default);
        }
    }
}