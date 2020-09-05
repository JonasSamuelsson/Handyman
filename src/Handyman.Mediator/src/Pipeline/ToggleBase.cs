using Handyman.Mediator.Pipeline.EventFilterToggle;
using Handyman.Mediator.Pipeline.EventHandlerToggle;
using Handyman.Mediator.Pipeline.RequestFilterToggle;
using Handyman.Mediator.Pipeline.RequestHandlerExperiment;
using Handyman.Mediator.Pipeline.RequestHandlerToggle;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public abstract class ToggleBase : IEventFilterToggle, IEventHandlerToggle, IRequestFilterToggle, IRequestHandlerToggle, IRequestHandlerExperimentToggle, IToggle
    {
        public virtual Task<bool> IsEnabled<TEvent>(EventFilterToggleMetadata toggleMetadata, EventContext<TEvent> eventContext)
            where TEvent : IEvent
        {
            return IsEnabled(toggleMetadata, GetMessageContext(eventContext));
        }

        public virtual Task<bool> IsEnabled<TEvent>(EventHandlerToggleMetadata toggleMetadata, EventContext<TEvent> eventContext)
            where TEvent : IEvent
        {
            return IsEnabled(toggleMetadata, GetMessageContext(eventContext));
        }

        public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetadata toggleMetadata, RequestContext<TRequest> requestContext)
            where TRequest : IRequest<TResponse>
        {
            return IsEnabled(toggleMetadata, GetMessageContext(requestContext));
        }

        public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerToggleMetadata toggleMetadata, RequestContext<TRequest> requestContext)
            where TRequest : IRequest<TResponse>
        {
            return IsEnabled(toggleMetadata, GetMessageContext(requestContext));
        }

        public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentMetadata experimentMetadata, RequestContext<TRequest> requestContext)
            where TRequest : IRequest<TResponse>
        {
            return IsEnabled(experimentMetadata, GetMessageContext(requestContext));
        }

        public abstract Task<bool> IsEnabled(IToggleMetadata toggleMetadata, MessageContext pipelineContext);

        private static MessageContext GetMessageContext<TEvent>(EventContext<TEvent> eventContext)
        {
            return new MessageContext
            {
                CancellationToken = eventContext.CancellationToken,
                Message = eventContext.Event,
                ServiceProvider = eventContext.ServiceProvider
            };
        }

        private static MessageContext GetMessageContext<TRequest>(RequestContext<TRequest> requestContext)
        {
            return new MessageContext
            {
                CancellationToken = requestContext.CancellationToken,
                Message = requestContext.Request,
                ServiceProvider = requestContext.ServiceProvider
            };
        }
    }
}