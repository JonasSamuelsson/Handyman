using System;
using System.Threading.Tasks;
using Handyman.Mediator.Internals;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class Experiment<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly Task<TResponse> _task;

        internal Experiment(ExperimentExecution<TRequest, TResponse> experimentExecution)
        {
            _task = experimentExecution.Task;
            Handler = experimentExecution.Handler;
            Duration = experimentExecution.Duration;
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