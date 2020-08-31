using Handyman.Mediator.Pipeline.EventFilterToggle;
using Handyman.Mediator.Pipeline.EventHandlerToggle;
using Handyman.Mediator.Pipeline.RequestFilterToggle;
using Handyman.Mediator.Pipeline.RequestHandlerExperiment;
using Handyman.Mediator.Pipeline.RequestHandlerToggle;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public abstract class ToggleBase : IEventFilterToggle, IEventHandlerToggle, IRequestFilterToggle, IRequestHandlerToggle, IRequestHandlerExperimentToggle
   {
      public virtual Task<bool> IsEnabled<TEvent>(EventFilterToggleMetadata toggleMetadata, EventContext<TEvent> eventContext)
          where TEvent : IEvent
      {
         return IsEnabled((IToggleMetadata)toggleMetadata, eventContext);
      }

      public virtual Task<bool> IsEnabled<TEvent>(EventHandlerToggleMetadata toggleMetadata, EventContext<TEvent> eventContext)
          where TEvent : IEvent
      {
         return IsEnabled((IToggleMetadata)toggleMetadata, eventContext);
      }

      public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetadata toggleMetadata, RequestContext<TRequest> requestContext)
          where TRequest : IRequest<TResponse>
      {
         return IsEnabled(toggleMetadata, requestContext);
      }

      public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerToggleMetadata toggleMetadata, RequestContext<TRequest> requestContext)
          where TRequest : IRequest<TResponse>
      {
         return IsEnabled(toggleMetadata, requestContext);
      }

      public virtual Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentMetadata experimentMetadata, RequestContext<TRequest> requestContext)
          where TRequest : IRequest<TResponse>
      {
         return IsEnabled(experimentMetadata, requestContext);
      }

      protected abstract Task<bool> IsEnabled(IToggleMetadata toggleMetadata, IPipelineContext pipelineContext);
   }
}