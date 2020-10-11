using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class Publisher<TEvent> : IPublisher<TEvent>
        where TEvent : IEvent
    {
        private readonly IMediator _mediator;

        public Publisher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Publish(TEvent @event, CancellationToken cancellationToken)
        {
            return _mediator.Publish(@event, cancellationToken);
        }
    }
}