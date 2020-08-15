using Handyman.Mediator.EventPipelineCustomization;
using Handyman.Mediator.RequestPipelineCustomization;
using System.Threading.Tasks;

namespace Handyman.Mediator.PipelineCustomization
{
    public abstract class ToggleBase : IEventFilterToggle, IEventHandlerToggle, IRequestFilterToggle, IRequestHandlerToggle
    {
        public virtual Task<bool> IsEnabled<TEvent>(EventFilterToggleMetaData toggleMetaData, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            return IsEnabled((IToggleMetaData)toggleMetaData, context);
        }

        public virtual Task<bool> IsEnabled<TEvent>(EventHandlerToggleMetaData toggleMetaData, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            return IsEnabled((IToggleMetaData)toggleMetaData, context);
        }

        public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetaData toggleMetaData, RequestPipelineContext<TRequest> context) where TRequest : IRequest<TResponse>
        {
            return IsEnabled(toggleMetaData, context);
        }

        public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerToggleMetaData toggleMetaData, RequestPipelineContext<TRequest> context) where TRequest : IRequest<TResponse>
        {
            return IsEnabled(toggleMetaData, context);
        }

        protected abstract Task<bool> IsEnabled(IToggleMetaData metaData, IPipelineContext context);
    }
}