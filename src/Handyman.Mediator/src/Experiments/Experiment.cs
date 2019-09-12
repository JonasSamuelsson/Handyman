using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.Experiments
{
    public class Experiment<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly Task<TResponse> _task;

        internal Experiment(Execution<TRequest, TResponse> execution)
        {
            _task = execution.Task;
            Handler = execution.Handler;
            Duration = execution.Duration;
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