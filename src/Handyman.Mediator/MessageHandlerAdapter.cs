using System;

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

        public void Handle(IEvent @event)
        {
            _handler.Handle((TEvent)@event);
        }
    }
}