using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestFilterToggleAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Type _toggleEnabledFilterType;

        public RequestFilterToggleAttribute(Type toggleEnabledFilterType)
        {
            _toggleEnabledFilterType = toggleEnabledFilterType ?? throw new ArgumentNullException(nameof(toggleEnabledFilterType));
        }

        public Type ToggleDisabledFilterType { get; set; }

        public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder, ServiceProvider serviceProvider)
        {
            builder.AddFilterSelector(new RequestFilterToggleFilterSelector<TRequest, TResponse>(_toggleEnabledFilterType)
            {
                ToggleDisabledFilterType = ToggleDisabledFilterType
            });
        }
    }
}