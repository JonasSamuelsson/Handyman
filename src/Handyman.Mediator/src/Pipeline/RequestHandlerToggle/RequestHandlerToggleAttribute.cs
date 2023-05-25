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
        public ToggleFailureMode? FailureMode { get; set; }

        public override async Task Execute<TRequest, TResponse>(RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext, RequestContext<TRequest> requestContext)
        {
            var metadata = _metadata.Value;
            var toggle = requestContext.ServiceProvider.GetRequiredService<IRequestHandlerToggle>();
            var toggleState = await toggle.IsEnabled<TRequest, TResponse>(metadata, requestContext).WithGloballyConfiguredAwait();

            PipelineBuilderUtilities.ApplyToggle(pipelineBuilderContext.Handlers, _toggleEnabledHandlerTypes, ToggleDisabledHandlerTypes, toggleState);
        }

        private RequestHandlerToggleMetadata CreateMetadata()
        {
            return new RequestHandlerToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledHandlerTypes = ToggleDisabledHandlerTypes ?? Type.EmptyTypes,
                ToggleEnabledHandlerTypes = _toggleEnabledHandlerTypes,
                FailureMode = FailureMode
            };
        }
    }
}