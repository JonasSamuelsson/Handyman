using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestHandlerToggle
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestHandlerToggleAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Lazy<RequestHandlerToggleMetadata> _metadata;
        private readonly Type[] _toggleEnabledHandlerTypes;

        public RequestHandlerToggleAttribute(Type toggleEnabledHandlerType)
            : this(new[] { toggleEnabledHandlerType })
        {
        }

        public RequestHandlerToggleAttribute(Type toggleEnabledHandlerType, Type toggleDisabledHandlerType)
            : this(new[] { toggleEnabledHandlerType }, new[] { toggleDisabledHandlerType })
        {
        }

        public RequestHandlerToggleAttribute(Type toggleEnabledHandlerType, Type[] toggleDisabledHandlerTypes)
            : this(new[] { toggleEnabledHandlerType }, toggleDisabledHandlerTypes)
        {
        }

        public RequestHandlerToggleAttribute(Type[] toggleEnabledHandlerTypes)
        {
            _metadata = new Lazy<RequestHandlerToggleMetadata>(CreateMetadata);
            _toggleEnabledHandlerTypes = toggleEnabledHandlerTypes;
        }

        public RequestHandlerToggleAttribute(Type[] toggleEnabledHandlerTypes, Type toggleDisabledHandlerType)
            : this(toggleEnabledHandlerTypes, new[] { toggleDisabledHandlerType })
        {
        }

        public RequestHandlerToggleAttribute(Type[] toggleEnabledHandlerTypes, Type[] toggleDisabledHandlerTypes)
            : this(toggleEnabledHandlerTypes)
        {
            ToggleDisabledHandlerTypes = toggleDisabledHandlerTypes;
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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestHandlerToggleAttribute<TToggleEnabledHandler> : RequestHandlerToggleAttribute
    {
        public RequestHandlerToggleAttribute() : base(typeof(TToggleEnabledHandler))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestHandlerToggleAttribute<TToggleEnabledHandler, TToggleDisabledHandler> : RequestHandlerToggleAttribute
    {
        public RequestHandlerToggleAttribute() : base(typeof(TToggleEnabledHandler), typeof(TToggleDisabledHandler))
        {
        }
    }
}