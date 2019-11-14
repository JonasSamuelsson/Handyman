using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventFilterToggle<TEvent>
        where TEvent : IEvent
    {
        Task<bool> IsEnabled(IEventPipelineContext<TEvent> context);
    }
}