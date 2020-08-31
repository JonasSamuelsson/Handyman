using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.EventHandlerToggle
{
    public interface IEventHandlerToggle
    {
        Task<bool> IsEnabled<TEvent>(EventHandlerToggleMetadata toggleMetadata, EventContext<TEvent> eventContext)
            where TEvent : IEvent;
    }
}