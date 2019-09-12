using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.Internals;

namespace Handyman.Mediator.Experiments
{
    internal class ExperimentRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly Type _baselineHandlerType;
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;
        private readonly IExperimentToggle<TRequest> _toggle;
        private readonly IExperimentEvaluator<TRequest, TResponse> _evaluator;

        public ExperimentRequestHandler(Type baselineHandlerType, IEnumerable<IRequestHandler<TRequest, TResponse>> handlers,
            IExperimentToggle<TRequest> toggle,
            IExperimentEvaluator<TRequest, TResponse> evaluator)
        {
            _baselineHandlerType = baselineHandlerType;
            _handlers = handlers.ToListOptimized();
            _toggle = toggle;
            _evaluator = evaluator;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var baselineHandler = _handlers.Single(x => x.GetType() == _baselineHandlerType);

            if (!await _toggle.IsEnabled(request, cancellationToken).ConfigureAwait(false))
                return await baselineHandler.Handle(request, cancellationToken).ConfigureAwait(false);

            var tasks = _handlers.Select(handler => Execute(handler, request, cancellationToken)).ToListOptimized();
            var executions = await Task.WhenAll(tasks).ConfigureAwait(false);
            var baselineExecution = executions.Single(x => x.Handler == baselineHandler);
            var baseline = new Baseline<TRequest, TResponse>(baselineExecution);
            var experiments = executions.Where(x => x != baselineExecution).Select(x => new Experiment<TRequest, TResponse>(x)).ToList();

            await _evaluator.Evaluate(request, baseline, experiments).ConfigureAwait(false);

            return await baselineExecution.Task;
        }

        private static Task<Execution<TRequest, TResponse>> Execute(IRequestHandler<TRequest, TResponse> handler, TRequest request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            return handler.Handle(request, cancellationToken)
                .ContinueWith(task =>
                {
                    var duration = stopwatch.Elapsed;
                    return new Execution<TRequest, TResponse>
                    {
                        Duration = duration,
                        Handler = handler,
                        Task = task
                    };
                }, TaskContinuationOptions.None);
        }
    }
}