using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestFilterToggle
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestFilterToggleAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Lazy<RequestFilterToggleMetadata> _metadata;
        private readonly Type[] _toggleEnabledFilterTypes;

        public RequestFilterToggleAttribute(Type toggleEnabledFilterType)
            : this(new[] { toggleEnabledFilterType })
        {
        }

        public RequestFilterToggleAttribute(Type toggleEnabledFilterType, Type toggleDisabledFilterType)
            : this(new[] { toggleEnabledFilterType }, new[] { toggleDisabledFilterType })
        {
        }

        public RequestFilterToggleAttribute(Type toggleEnabledFilterType, Type[] toggleDisabledFilterTypes)
            : this(new[] { toggleEnabledFilterType }, toggleDisabledFilterTypes)
        {
        }

        public RequestFilterToggleAttribute(Type[] toggleEnabledFilterTypes)
        {
            _metadata = new Lazy<RequestFilterToggleMetadata>(CreateMetadata);
            _toggleEnabledFilterTypes = toggleEnabledFilterTypes;
        }

        public RequestFilterToggleAttribute(Type[] toggleEnabledFilterTypes, Type toggleDisabledFilterType)
            : this(toggleEnabledFilterTypes, new[] { toggleDisabledFilterType })
        {
        }

        public RequestFilterToggleAttribute(Type[] toggleEnabledFilterTypes, Type[] toggleDisabledFilterTypes)
            : this(toggleEnabledFilterTypes)
        {
            ToggleDisabledFilterTypes = toggleDisabledFilterTypes;
        }

        public string? Name { get; set; }
        public string[]? Tags { get; set; }
        public Type[]? ToggleDisabledFilterTypes { get; set; }
        public ToggleFailureMode? FailureMode { get; set; }

        public override async Task Execute<TRequest, TResponse>(RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext, RequestContext<TRequest> requestContext)
        {
            var metadata = _metadata.Value;
            var toggle = requestContext.ServiceProvider.GetRequiredService<IRequestFilterToggle>();
            var toggleState = await toggle.IsEnabled<TRequest, TResponse>(metadata, requestContext).WithGloballyConfiguredAwait();

            PipelineBuilderUtilities.ApplyToggle(pipelineBuilderContext.Filters, _toggleEnabledFilterTypes, ToggleDisabledFilterTypes, toggleState);
        }

        private RequestFilterToggleMetadata CreateMetadata()
        {
            return new RequestFilterToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledFilterTypes = ToggleDisabledFilterTypes ?? Type.EmptyTypes,
                ToggleEnabledFilterTypes = _toggleEnabledFilterTypes,
                FailureMode = FailureMode
            };
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestFilterToggleAttribute<TToggleEnabledFilter> : RequestFilterToggleAttribute
    {
        public RequestFilterToggleAttribute() : base(typeof(TToggleEnabledFilter))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestFilterToggleAttribute<TToggleEnabledFilter, TToggleDisabledFilter> : RequestFilterToggleAttribute
    {
        public RequestFilterToggleAttribute() : base(typeof(TToggleEnabledFilter), typeof(TToggleDisabledFilter))
        {
        }
    }
}