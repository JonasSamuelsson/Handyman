using Handyman.Mediator.EventPipelineCustomization;
using Handyman.Mediator.RequestPipelineCustomization;
using System.Threading.Tasks;

namespace Handyman.Mediator.PipelineCustomization
{
    public abstract class ToggleBase : IEventFilterToggle, IEventHandlerToggle, IRequestFilterToggle, IRequestHandlerToggle, IRequestHandlerExperimentToggle
    {
        public virtual Task<bool> IsEnabled<TEvent>(EventFilterToggleMetadata toggleMetadata, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            return IsEnabled((IToggleMetadata)toggleMetadata, context);
        }

        public virtual Task<bool> IsEnabled<TEvent>(EventHandlerToggleMetadata toggleMetadata, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            return IsEnabled((IToggleMetadata)toggleMetadata, context);
        }

        public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetadata toggleMetadata, RequestPipelineContext<TRequest> context) where TRequest : IRequest<TResponse>
        {
            return IsEnabled(toggleMetadata, context);
        }

        public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerToggleMetadata toggleMetadata, RequestPipelineContext<TRequest> context) where TRequest : IRequest<TResponse>
        {
            return IsEnabled(toggleMetadata, context);
        }

        public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentMetadata experimentMetadata,
            RequestPipelineContext<TRequest> context) where TRequest : IRequest<TResponse>
        {
            return IsEnabled(experimentMetadata, context);
        }

        protected abstract Task<bool> IsEnabled(IToggleMetadata metadata, IPipelineContext context);
    }
}