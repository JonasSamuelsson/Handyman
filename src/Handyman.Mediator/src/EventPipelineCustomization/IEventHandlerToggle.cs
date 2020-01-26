using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventHandlerToggle
    {
        Task<bool> IsEnabled<TEvent>(EventHandlerToggleInfo toggleInfo, EventPipelineContext<TEvent> context)
            where TEvent : IEvent;
    }
}