using System.Threading.Tasks;
using Handyman.Mediator.Pipeline;

namespace Handyman.Mediator
{
    public interface IEventFilter<TEvent>
        where TEvent : IEvent
    {
        Task Execute(EventPipelineContext<TEvent> context, EventFilterExecutionDelegate next);
    }
}