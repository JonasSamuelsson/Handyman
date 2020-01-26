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
        public string ToggleName { get; set; }

        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            var toggleInfo = new RequestFilterToggleInfo
            {
                ToggleDisabledFilterType = ToggleDisabledFilterType,
                ToggleEnabledFilterType = _toggleEnabledFilterType,
                ToggleName = ToggleName
            };

            builder.AddFilterSelector(new RequestFilterToggleFilterSelector(toggleInfo));
        }
    }
}