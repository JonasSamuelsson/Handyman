using System.Threading.Tasks;

namespace Handyman.Dispatch
{
    public interface IMessageHandler<TMessage>
       where TMessage : IMessage
    {
        Task Handle(TMessage message);
    }
}