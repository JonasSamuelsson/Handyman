using System;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    internal class AsyncMessageHandlerAdapter<TMessage> : IAsyncMessageHandler<IAsyncMessage>
        where TMessage : IAsyncMessage
    {
        private readonly IAsyncMessageHandler<TMessage> _handler;

        public AsyncMessageHandlerAdapter(IAsyncMessageHandler<TMessage> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task Handle(IAsyncMessage message)
        {
            return _handler.Handle((TMessage)message);
        }
    }
}