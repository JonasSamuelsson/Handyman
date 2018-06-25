using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IEventHandler<TEvent>
       where TEvent : IEvent
    {
        Task Handle(TEvent @event);
    }
}