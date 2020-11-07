using System;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestHandlerToggle
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestHandlerToggleAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Lazy<RequestHandlerToggleMetadata> _metadata;
        private readonly Type[] _toggleEnabledHandlerTypes;

        public RequestHandlerToggleAttribute(Type toggleEnabledHandlerType)
            : this(new[] { toggleEnabledHandlerType ?? throw new ArgumentNullException(nameof(toggleEnabledHandlerType)) })
        {
        }

        public RequestHandlerToggleAttribute(Type[] toggleEnabledHandlerTypes)
        {
            if (toggleEnabledHandlerTypes == null)
                throw new ArgumentNullException(nameof(toggleEnabledHandlerTypes));

            if (!toggleEnabledHandlerTypes.Any())
                throw new ArgumentException();

            _metadata = new Lazy<RequestHandlerToggleMetadata>(CreateMetadata);
            _toggleEnabledHandlerTypes = toggleEnabledHandlerTypes;
        }

        public string? Name { get; set; }
        public string[]? Tags { get; set; }
        public Type[]? ToggleDisabledHandlerTypes { get; set; }

        public override async Task Execute<TRequest, TResponse>(RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext, RequestContext<TRequest> requestContext)
        {
            var metadata = _metadata.Value;

            var toggle = requestContext.ServiceProvider.GetRequiredService<IRequestHandlerToggle>();
            var enabled = await toggle.IsEnabled<TRequest, TResponse>(metadata, requestContext).WithGloballyConfiguredAwait();

            if (!enabled)
            {
                pipelineBuilderContext.Handlers.RemoveAll(x => metadata.ToggleEnabledHandlerTypes.Contains(x.GetType()));
            }
            else if (metadata.ToggleDisabledHandlerTypes.Any())
            {
                pipelineBuilderContext.Handlers.RemoveAll(x => metadata.ToggleDisabledHandlerTypes.Contains(x.GetType()));
            }
        }

        private RequestHandlerToggleMetadata CreateMetadata()
        {
            return new RequestHandlerToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledHandlerTypes = ToggleDisabledHandlerTypes ?? Type.EmptyTypes,
                ToggleEnabledHandlerTypes = _toggleEnabledHandlerTypes
            };
        }
    }
}