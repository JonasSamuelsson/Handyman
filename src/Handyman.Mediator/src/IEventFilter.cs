using System.Threading.Tasks;
using Handyman.Mediator.EventPipelineCustomization;

namespace Handyman.Mediator
{
    public interface IEventFilter<TEvent>
        where TEvent : IEvent
    {
        Task Execute(IEventPipelineContext<TEvent> context, EventFilterExecutionDelegate next);
    }
}