using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IAsyncMessageHandler<TMessage>
        where TMessage : IAsyncMessage
    {
        Task Handle(TMessage message);
    }
}