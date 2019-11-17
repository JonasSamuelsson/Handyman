using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IEventFilter<TEvent>
        where TEvent : IEvent
    {
        Task Execute(EventPipelineContext<TEvent> context, EventFilterExecutionDelegate next);
    }
}