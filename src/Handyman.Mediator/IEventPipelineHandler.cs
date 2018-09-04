using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IEventPipelineHandler<TEvent>
        where TEvent : IEvent
    {
        IEnumerable<Task> Handle(TEvent @event, CancellationToken cancellationToken, Func<TEvent, CancellationToken, IEnumerable<Task>> next);
    }
}