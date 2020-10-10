using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class Publisher<TEvent> : IPublisher<TEvent>
        where TEvent : IEvent
    {
        private readonly IPublisher _publisher;

        public Publisher(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task Publish(TEvent @event, CancellationToken cancellationToken)
        {
            return _publisher.Publish(@event, cancellationToken);
        }
    }
}