using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestFilterToggleAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Type _toggledFilterType;

        public RequestFilterToggleAttribute(Type toggledFilterType)
        {
            _toggledFilterType = toggledFilterType ?? throw new ArgumentNullException(nameof(toggledFilterType));
        }

        public Type FallbackFilterType { get; set; }

        public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder, ServiceProvider serviceProvider)
        {
            builder.AddFilterSelector(new RequestFilterToggleFilterSelector<TRequest, TResponse>(_toggledFilterType)
            {
                FallbackFilterType = FallbackFilterType
            });
        }
    }
}