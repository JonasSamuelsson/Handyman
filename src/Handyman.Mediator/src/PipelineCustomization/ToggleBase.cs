using Handyman.Mediator.EventPipelineCustomization;
using System.Threading.Tasks;

namespace Handyman.Mediator.PipelineCustomization
{
    public abstract class ToggleBase : IEventFilterToggle, IEventHandlerToggle
    {
        public virtual Task<bool> IsEnabled<TEvent>(EventFilterToggleMetaData toggleMetaData, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            return IsEnabled((IToggleMetaData)toggleMetaData, context);
        }

        public virtual Task<bool> IsEnabled<TEvent>(EventHandlerToggleMetaData toggleMetaData, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            return IsEnabled((IToggleMetaData)toggleMetaData, context);
        }

        protected abstract Task<bool> IsEnabled(IToggleMetaData metaData, IPipelineContext context);
    }
}