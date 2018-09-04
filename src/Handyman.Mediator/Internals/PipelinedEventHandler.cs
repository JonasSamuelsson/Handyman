using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal class PipelinedEventHandler<TEvent> : EventHandler<TEvent>
        where TEvent : IEvent
    {
        private int _index;
        private readonly IEventPipelineHandler<TEvent>[] _pipelineHandlers;

        public PipelinedEventHandler(IEnumerable<IEventPipelineHandler<TEvent>> pipelineHandlers, IEnumerable<IEventHandler<TEvent>> handlers)
            : base(handlers)
        {
            _pipelineHandlers = pipelineHandlers?.ToArray() ?? throw new ArgumentNullException(nameof(pipelineHandlers));
        }

        protected override IEnumerable<Task> Execute(TEvent @event, CancellationToken cancellationToken)
        {
            var index = _index++;

            if (index == _pipelineHandlers.Length)
                return base.Execute(@event, cancellationToken);

            var pipelineHandler = _pipelineHandlers[index];
            return pipelineHandler.Handle(@event, cancellationToken, Execute);
        }
    }
}