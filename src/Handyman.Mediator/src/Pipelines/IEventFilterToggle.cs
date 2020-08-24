using System.Threading.Tasks;

namespace Handyman.Mediator.Pipelines
{
    public interface IEventFilterToggle
    {
        Task<bool> IsEnabled<TEvent>(EventFilterToggleMetadata toggleMetadata,
            EventPipelineContext<TEvent> pipelineContext)
            where TEvent : IEvent;
    }
}