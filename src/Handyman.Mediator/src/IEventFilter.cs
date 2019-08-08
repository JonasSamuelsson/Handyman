using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IEventFilter<TEvent>
        where TEvent : IEvent
    {
        Task Execute(EventFilterContext<TEvent> context, EventFilterExecutionDelegate next);
    }
}