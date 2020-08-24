using System.Threading.Tasks;

namespace Handyman.Mediator.Pipelines
{
    public abstract class ToggleBase : IEventFilterToggle, IEventHandlerToggle, IRequestFilterToggle, IRequestHandlerToggle, IRequestHandlerExperimentToggle
    {
        public virtual Task<bool> IsEnabled<TEvent>(EventFilterToggleMetadata toggleMetadata,
            EventPipelineContext<TEvent> pipelineContext) where TEvent : IEvent
        {
            return IsEnabled((IToggleMetadata)toggleMetadata, pipelineContext);
        }

        public virtual Task<bool> IsEnabled<TEvent>(EventHandlerToggleMetadata toggleMetadata,
            EventPipelineContext<TEvent> pipelineContext) where TEvent : IEvent
        {
            return IsEnabled((IToggleMetadata)toggleMetadata, pipelineContext);
        }

        public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetadata toggleMetadata,
            RequestPipelineContext<TRequest> pipelineContext) where TRequest : IRequest<TResponse>
        {
            return IsEnabled(toggleMetadata, pipelineContext);
        }

        public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerToggleMetadata toggleMetadata,
            RequestPipelineContext<TRequest> pipelineContext) where TRequest : IRequest<TResponse>
        {
            return IsEnabled(toggleMetadata, pipelineContext);
        }

        public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentMetadata experimentMetadata,
            RequestPipelineContext<TRequest> pipelineContext) where TRequest : IRequest<TResponse>
        {
            return IsEnabled(experimentMetadata, pipelineContext);
        }

        protected abstract Task<bool> IsEnabled(IToggleMetadata toggleMetadata, IPipelineContext pipelineContext);
    }
}