using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventHandlerToggle
    {
        Task<bool> IsEnabled<TEvent>(EventHandlerToggleMetadata toggleMetadata, EventPipelineContext<TEvent> context)
            where TEvent : IEvent;
    }
}