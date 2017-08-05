using System;
using System.Threading.Tasks;

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

        public Task Handle(IMessage message)
        {
            return _handler.Handle((TMessage)message);
        }
    }
}