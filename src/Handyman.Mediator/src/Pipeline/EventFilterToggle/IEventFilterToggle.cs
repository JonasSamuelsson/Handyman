using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.EventFilterToggle
{
    public interface IEventFilterToggle
    {
        Task<bool> IsEnabled<TEvent>(EventFilterToggleMetadata toggleMetadata, EventPipelineContext<TEvent> pipelineContext)
            where TEvent : IEvent;
    }
}