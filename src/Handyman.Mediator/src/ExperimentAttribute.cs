using Handyman.Mediator.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExperimentAttribute : RequestHandlerProviderAttribute
    {
        public ExperimentAttribute(Type baselineHandlerType)
        {
            BaselineHandlerType = baselineHandlerType;
        }

        public Type BaselineHandlerType { get; }
        public double? ToggleRate { get; set; }
        public Type ToggleType { get; set; }

        public override IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider)
        {
            var handlers = GetHandlers<TRequest, TResponse>(serviceProvider);
            var toggle = GetToggle<TRequest>();
            var evaluators = GetEvaluators<TRequest, TResponse>(serviceProvider);

            return new ExperimentRequestHandler<TRequest, TResponse>(BaselineHandlerType, handlers, toggle, evaluators);
        }

        private IExperimentToggle<TRequest> GetToggle<TRequest>()
        {
            return new ExperimentToggle<TRequest>(ToggleRate ?? 1);
        }

        private static IEnumerable<IRequestHandler<TRequest, TResponse>> GetHandlers<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            var type = typeof(IEnumerable<IRequestHandler<TRequest, TResponse>>);
            return (IEnumerable<IRequestHandler<TRequest, TResponse>>)serviceProvider.Invoke(type);
        }

        private static IEnumerable<IExperimentEvaluator<TRequest, TResponse>> GetEvaluators<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            var type = typeof(IEnumerable<IExperimentEvaluator<TRequest, TResponse>>);
            return (IEnumerable<IExperimentEvaluator<TRequest, TResponse>>)serviceProvider.Invoke(type);
        }
    }

    internal class ExperimentRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly Type _baselineHandlerType;
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;
        private readonly IExperimentToggle<TRequest> _toggle;
        private readonly IEnumerable<IExperimentEvaluator<TRequest, TResponse>> _evaluators;

        public ExperimentRequestHandler(Type baselineHandlerType, IEnumerable<IRequestHandler<TRequest, TResponse>> handlers,
            IExperimentToggle<TRequest> toggle,
            IEnumerable<IExperimentEvaluator<TRequest, TResponse>> evaluators)
        {
            _baselineHandlerType = baselineHandlerType;
            _handlers = handlers.ToListOptimized();
            _toggle = toggle;
            _evaluators = evaluators.ToListOptimized();
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var baselineHandler = _handlers.Single(x => x.GetType() == _baselineHandlerType);

            if (!await _toggle.IsEnabled(request, cancellationToken).ConfigureAwait(false))
                return await baselineHandler.Handle(request, cancellationToken).ConfigureAwait(false);

            var tasks = _handlers.Select(handler => Execute(handler, request, cancellationToken)).ToListOptimized();
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            var baselineResult = results.Single(x => x.Handler == baselineHandler);
            var baseline = new Baseline<TRequest, TResponse>(baselineResult);
            var experiments = results.Where(x => x != baselineResult).Select(x => new Experiment<TRequest, TResponse>(x)).ToList();

            foreach (var evaluator in _evaluators)
            {
                await evaluator.Evaluate(request, baseline, experiments).ConfigureAwait(false);
            }

            return await baselineResult.Task;
        }

        private Task<Result> Execute(IRequestHandler<TRequest, TResponse> handler, TRequest request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            return handler.Handle(request, cancellationToken)
                .ContinueWith(task =>
                {
                    var duration = stopwatch.Elapsed;
                    return new Result
                    {
                        Duration = duration,
                        Handler = handler,
                        Task = task
                    };
                });
        }

        internal class Result
        {
            public TimeSpan Duration { get; set; }
            public IRequestHandler<TRequest, TResponse> Handler { get; set; }
            public Task<TResponse> Task { get; set; }
        }
    }

    internal class ExperimentToggle<TRequest> : IExperimentToggle<TRequest>
    {
        private static readonly Random Random = new Random();

        private readonly double _rate;

        public ExperimentToggle(double rate)
        {
            _rate = rate;
        }

        public Task<bool> IsEnabled(TRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(((-_rate) + 1) <= Random.NextDouble());
        }
    }

    public interface IExperimentEvaluator<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task Evaluate(TRequest request, Experiment<TRequest, TResponse> baseline, IEnumerable<Experiment<TRequest, TResponse>> experiments);
    }

    internal interface IExperimentToggle<TRequest>
    {
        Task<bool> IsEnabled(TRequest request, CancellationToken cancellationToken);
    }

    public class Baseline<TRequest, TResponse> : Experiment<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        internal Baseline(ExperimentRequestHandler<TRequest, TResponse>.Result result)
            : base(result)
        {
        }
    }

    public class Experiment<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly Task<TResponse> _task;

        internal Experiment(ExperimentRequestHandler<TRequest, TResponse>.Result result)
        {
            _task = result.Task;
            Handler = result.Handler;
            Duration = result.Duration;
        }

        public IRequestHandler<TRequest, TResponse> Handler { get; }
        public TResponse Response => RanToCompletion ? _task.Result : default;
        public Exception Exception => _task.Exception;
        public TimeSpan Duration { get; }
        public bool RanToCompletion => _task.Status == TaskStatus.RanToCompletion;
        public bool Faulted => _task.IsFaulted;
        public bool Canceled => _task.IsCanceled;
    }
}