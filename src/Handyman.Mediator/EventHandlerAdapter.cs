using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    internal class EventHandlerAdapter<TEvent> : IEventHandler<IEvent>
       where TEvent : IEvent
    {
        private readonly IEventHandler<TEvent> _handler;

        public EventHandlerAdapter(IEventHandler<TEvent> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task Handle(IEvent @event, CancellationToken cancellationToken)
        {
            return _handler.Handle((TEvent)@event, cancellationToken);
        }
    }
}