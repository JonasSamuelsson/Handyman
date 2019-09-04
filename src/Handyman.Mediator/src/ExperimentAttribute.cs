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
        public ExperimentAttribute(Type primaryHandlerType)
        {
            PrimaryHandlerType = primaryHandlerType;
        }

        public Type PrimaryHandlerType { get; }

        public override IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider)
        {
            var handlers = GetHandlers<TRequest, TResponse>(serviceProvider);
            var evaluators = GetEvaluators<TRequest, TResponse>(serviceProvider);

            return new RequestHandler<TRequest, TResponse>(PrimaryHandlerType, handlers, evaluators);
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

        private class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
        {
            private readonly Type _primaryHandlerType;
            private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;
            private readonly IEnumerable<IExperimentEvaluator<TRequest, TResponse>> _evaluators;

            public RequestHandler(Type primaryHandlerType, IEnumerable<IRequestHandler<TRequest, TResponse>> handlers,
                IEnumerable<IExperimentEvaluator<TRequest, TResponse>> evaluators)
            {
                _primaryHandlerType = primaryHandlerType;
                _handlers = handlers.ToListOptimized();
                _evaluators = evaluators.ToListOptimized();
            }

            public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
            {
                var primaryHandler = _handlers.Single(x => x.GetType() == _primaryHandlerType);
                var experiments = _handlers.Select(handler => Execute(handler, request, cancellationToken)).ToListOptimized();
            }

            private void Execute(IRequestHandler<TRequest, TResponse> handler, TRequest request, CancellationToken cancellationToken)
            {
                var stopwatch = Stopwatch.StartNew();
                handler.Handle(request, cancellationToken)
                    .ContinueWith(task =>
                    {
                        var duration = stopwatch.Elapsed;
                        return new Experiment<TRequest, TResponse>(handler, task, duration);
                    })
                    .ConfigureAwait(false);
            }
        }
    }

    public interface IExperimentEvaluator<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task Evaluate(TRequest request, Experiment<TRequest, TResponse> primary, IEnumerable<Experiment<TRequest, TResponse>> experiments);
    }

    public class Experiment<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly Task<TResponse> _task;

        public Experiment(IRequestHandler<TRequest, TResponse> handler, Task<TResponse> task, TimeSpan duration)
        {
            Handler = handler;
            _task = task;
            Duration = duration;
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