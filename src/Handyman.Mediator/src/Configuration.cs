using Handyman.Mediator.Internals;

namespace Handyman.Mediator
{
    public class Configuration
    {
        /// <summary>
        /// Enable/disable event pipeline, default is false.
        /// </summary>
        public bool EventPipelineEnabled { get; set; } = false;

        public IEventHandlerProvider EventHandlerProvider { get; set; }
        public IEventFilterProvider EventFilterProvider { get; set; }

        /// <summary>
        /// Enable/disable request pipeline, default is false.
        /// </summary>
        public bool RequestPipelineEnabled { get; set; } = false;

        public IRequestHandlerProvider RequestHandlerProvider { get; set; }
        public IRequestPipelineHandlerProvider RequestPipelineHandlerProvider { get; set; }

        internal IEventHandlerProvider GetEventHandlerProvider()
        {
            if (EventHandlerProvider != null)
                return EventHandlerProvider;

            return Internals.EventHandlerProvider.Instance;
        }

        internal IEventFilterProvider GetEventPipelineHandlerProvider()
        {
            if (EventFilterProvider != null)
                return EventFilterProvider;

            if (EventPipelineEnabled)
                return Handyman.Mediator.EventFilterProvider.Instance;

            return NoEventFilterProvider.Instance;
        }

        internal IRequestHandlerProvider GetRequestHandlerProvider()
        {
            if (RequestHandlerProvider != null)
                return RequestHandlerProvider;

            return Internals.RequestHandlerProvider.Instance;
        }

        internal IRequestPipelineHandlerProvider GetRequestPipelineHandlerProvider()
        {
            if (RequestPipelineHandlerProvider != null)
                return RequestPipelineHandlerProvider;

            if (RequestPipelineEnabled)
                return Handyman.Mediator.RequestPipelineHandlerProvider.Instance;

            return NoRequestPipelineHandlerProvider.Instance;
        }
    }
}