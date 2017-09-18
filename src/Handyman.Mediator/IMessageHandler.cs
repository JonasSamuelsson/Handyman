namespace Handyman.Mediator
{
    public interface IMessageHandler<TMessage>
       where TMessage : IMessage
    {
        void Handle(TMessage message);
    }
}