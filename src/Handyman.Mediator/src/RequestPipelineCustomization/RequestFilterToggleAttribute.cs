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

        public override bool PipelineCanBeReused => true;
        public Type ToggleDisabledFilterType { get; set; }

        public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder,
            IServiceProvider serviceProvider)
        {
            builder.AddFilterSelector(new RequestFilterToggleFilterSelector(_toggleEnabledFilterType)
            {
                ToggleDisabledFilterType = ToggleDisabledFilterType
            });
        }
    }
}