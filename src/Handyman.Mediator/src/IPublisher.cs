using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IPublisher
    {
        Task Publish(IEvent @event, CancellationToken cancellationToken);
    }

    public interface IPublisher<TEvent>
        where TEvent : IEvent
    {
        Task Publish(TEvent @event, CancellationToken cancellationToken);
    }
}