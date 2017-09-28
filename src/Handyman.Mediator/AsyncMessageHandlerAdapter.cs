using System;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    internal class AsyncEventHandlerAdapter<TEvent> : IAsyncEventHandler<IAsyncEvent>
        where TEvent : IAsyncEvent
    {
        private readonly IAsyncEventHandler<TEvent> _handler;

        public AsyncEventHandlerAdapter(IAsyncEventHandler<TEvent> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task Handle(IAsyncEvent @event)
        {
            return _handler.Handle((TEvent)@event);
        }
    }
}