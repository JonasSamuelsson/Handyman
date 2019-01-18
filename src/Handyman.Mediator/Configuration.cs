using Handyman.Mediator.Internals;

namespace Handyman.Mediator
{
    public class Configuration
    {
        /// <summary>
        /// Enable/disable event pipeline, default is false.
        /// </summary>
        public bool EventPipelineEnabled { get; set; } = false;

        /// <summary>
        /// Enable/disable request pipeline, default is false.
        /// </summary>
        public bool RequestPipelineEnabled { get; set; } = false;

        public IRequestHandlerProvider RequestHandlerProvider { get; set; }
        public IRequestPipelineHandlerProvider RequestPipelineHandlerProvider { get; set; }

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
                return Internals.RequestPipelineHandlerProvider.Instance;

            return NoRequestPipelineHandlerProvider.Instance;
        }
    }
}