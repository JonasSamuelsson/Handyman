using System;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestFilterToggle
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestFilterToggleAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Lazy<RequestFilterToggleMetadata> _metadata;
        private readonly Type[] _toggleEnabledFilterTypes;

        public RequestFilterToggleAttribute(Type toggleEnabledFilterType)
            : this(new[] { toggleEnabledFilterType ?? throw new ArgumentNullException(nameof(toggleEnabledFilterType)) })
        {
        }

        public RequestFilterToggleAttribute(Type[] toggleEnabledFilterTypes)
        {
            if (toggleEnabledFilterTypes == null)
                throw new ArgumentNullException(nameof(toggleEnabledFilterTypes));

            if (!toggleEnabledFilterTypes.Any())
                throw new ArgumentException();

            _metadata = new Lazy<RequestFilterToggleMetadata>(CreateMetadata);
            _toggleEnabledFilterTypes = toggleEnabledFilterTypes;
        }

        public string? Name { get; set; }
        public string[]? Tags { get; set; }
        public Type[]? ToggleDisabledFilterTypes { get; set; }

        public override async Task Execute<TRequest, TResponse>(RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext, RequestContext<TRequest> requestContext)
        {
            var metadata = _metadata.Value;

            var toggle = requestContext.ServiceProvider.GetRequiredService<IRequestFilterToggle>();
            var enabled = await toggle.IsEnabled<TRequest, TResponse>(metadata, requestContext).WithGloballyConfiguredAwait();

            if (!enabled)
            {
                pipelineBuilderContext.Filters.RemoveAll(x => metadata.ToggleEnabledFilterTypes.Contains(x.GetType()));
            }
            else if (metadata.ToggleDisabledFilterTypes.Any())
            {
                pipelineBuilderContext.Filters.RemoveAll(x => metadata.ToggleDisabledFilterTypes.Contains(x.GetType()));
            }
        }

        private RequestFilterToggleMetadata CreateMetadata()
        {
            return new RequestFilterToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledFilterTypes = ToggleDisabledFilterTypes ?? Type.EmptyTypes,
                ToggleEnabledFilterTypes = _toggleEnabledFilterTypes
            };
        }
    }
}