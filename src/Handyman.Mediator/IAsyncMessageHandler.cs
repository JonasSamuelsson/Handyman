using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IAsyncEventHandler<TEvent>
        where TEvent : IAsyncEvent
    {
        Task Handle(TEvent @event);
    }
}