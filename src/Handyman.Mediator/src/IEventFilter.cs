using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IEventFilter<TEvent>
        where TEvent : IEvent
    {
        Task Execute(TEvent @event, CancellationToken cancellationToken, Func<TEvent, CancellationToken, Task> next);
    }
}