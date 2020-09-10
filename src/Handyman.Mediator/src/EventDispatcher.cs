using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class EventDispatcher<TEvent> : IEventDispatcher<TEvent>
        where TEvent : IEvent
    {
        private readonly IMediator _mediator;

        public EventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Publish(TEvent @event, CancellationToken cancellationToken)
        {
            return _mediator.Publish(@event, cancellationToken);
        }
    }
}