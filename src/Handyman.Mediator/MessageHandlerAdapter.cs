using System.Threading.Tasks;

namespace Handyman.Mediator
{
    internal class MessageHandlerAdapter<TMessage> : IMessageHandler<IMessage>
       where TMessage : IMessage
    {
        private readonly IMessageHandler<TMessage> _messageHandler;

        public MessageHandlerAdapter(IMessageHandler<TMessage> messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public Task Handle(IMessage message)
        {
            return _messageHandler.Handle((TMessage)message);
        }
    }
}