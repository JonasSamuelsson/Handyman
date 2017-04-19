using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IMessageHandler<TMessage>
       where TMessage : IMessage
    {
        Task Handle(TMessage message);
    }
}