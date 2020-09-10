using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IEventDispatcher<TEvent>
        where TEvent : IEvent
    {
        Task Publish(TEvent @event, CancellationToken cancellationToken);
    }
}