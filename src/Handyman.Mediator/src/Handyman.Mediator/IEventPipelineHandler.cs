using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IEventPipelineHandler<TEvent>
        where TEvent : IEvent
    {
        Task Handle(TEvent @event, CancellationToken cancellationToken, Func<TEvent, CancellationToken, Task> next);
    }
}