using System;

namespace Handyman.Mediator
{
    internal class MessageHandlerAdapter<TMessage> : IMessageHandler<IMessage>
       where TMessage : IMessage
    {
        private readonly IMessageHandler<TMessage> _handler;

        public MessageHandlerAdapter(IMessageHandler<TMessage> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void Handle(IMessage message)
        {
            _handler.Handle((TMessage)message);
        }
    }
}