using Handyman.Mediator.EventPipelineCustomization;
using System.Threading.Tasks;

namespace Handyman.Mediator.PipelineCustomization
{
    public abstract class ToggleBase : IEventFilterToggle
    {
        public virtual Task<bool> IsEnabled<TEvent>(EventFilterToggleMetaData toggleMetaData, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            return IsEnabled((IToggleMetaData)toggleMetaData, context);
        }

        protected abstract Task<bool> IsEnabled(IToggleMetaData metaData, IPipelineContext context);
    }
}