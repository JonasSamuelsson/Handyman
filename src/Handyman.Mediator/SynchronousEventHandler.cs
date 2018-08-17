using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public abstract class SynchronousEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
    {
        Task IEventHandler<TEvent>.Handle(TEvent @event, CancellationToken cancellationToken)
        {
            Handle(@event);
            return Task.CompletedTask;
        }

        protected abstract void Handle(TEvent @event);
    }
}