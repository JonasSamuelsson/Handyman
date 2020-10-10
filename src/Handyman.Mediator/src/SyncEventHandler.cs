using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public abstract class SyncEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
    {
        Task IEventHandler<TEvent>.Handle(TEvent @event, CancellationToken cancellationToken)
        {
            Handle(@event, cancellationToken);
            return Task.CompletedTask;
        }

        protected abstract void Handle(TEvent @event, CancellationToken cancellationToken);
    }
}