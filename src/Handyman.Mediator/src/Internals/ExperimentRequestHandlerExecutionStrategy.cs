using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.RequestPipelineCustomization;

namespace Handyman.Mediator.Internals
{
    internal class ExperimentRequestHandlerExecutionStrategy<TRequest, TResponse> : IRequestHandlerExecutionStrategy<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly Type _baselineHandlerType;

        public ExperimentRequestHandlerExecutionStrategy(Type baselineHandlerType)
        {
            _baselineHandlerType = baselineHandlerType;
        }

        public async Task<TResponse> Execute(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
        {
            var baselineHandler = GetBaselineHandler(handlers);

            var toggle = context.ServiceProvider.GetRequiredService<IExperimentToggle<TRequest>>();

            var request = context.Request;
            var cancellationToken = context.CancellationToken;

            if (!await toggle.IsEnabled(request, cancellationToken).ConfigureAwait(false))
            {
                return await baselineHandler.Handle(request, cancellationToken).ConfigureAwait(false);
            }

            var evaluator = context.ServiceProvider.GetRequiredService<IExperimentEvaluator<TRequest, TResponse>>();
            var tasks = handlers.Select(handler => Execute(handler, request, cancellationToken)).ToListOptimized();
            var executions = await Task.WhenAll(tasks).ConfigureAwait(false);
            var baselineExecution = executions.Single(x => x.Handler == baselineHandler);
            var baseline = new ExperimentBaseline<TRequest, TResponse>(baselineExecution);
            var experiments = executions.Where(x => x != baselineExecution).Select(x => new Experiment<TRequest, TResponse>(x)).ToList();

            await evaluator.Evaluate(request, baseline, experiments).ConfigureAwait(false);

            return await baselineExecution.Task;
        }

        private IRequestHandler<TRequest, TResponse> GetBaselineHandler(List<IRequestHandler<TRequest, TResponse>> handlers)
        {
            IRequestHandler<TRequest, TResponse> baselineHandler = null;

            foreach (var handler in handlers)
            {
                if (handler.GetType() != _baselineHandlerType)
                    continue;

                if (baselineHandler != null)
                    throw new InvalidOperationException($"{typeof(TRequest).FullName} has multiple experiment baseline handlers.");

                baselineHandler = handler;
            }

            if (baselineHandler == null)
                throw new InvalidOperationException($"{typeof(TRequest).FullName} does not have any experiment baseline handlers.");

            return baselineHandler;
        }

        private static Task<ExperimentExecution<TRequest, TResponse>> Execute(IRequestHandler<TRequest, TResponse> handler, TRequest request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            return handler.Handle(request, cancellationToken)
                .ContinueWith(task =>
                {
                    var duration = stopwatch.Elapsed;
                    return new ExperimentExecution<TRequest, TResponse>
                    {
                        Duration = duration,
                        Handler = handler,
                        Task = task
                    };
                }, TaskContinuationOptions.None);
        }
    }
}