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

        public string Name { get; set; }
        public string[] Tags { get; set; }
        public Type ToggleDisabledFilterType { get; set; }

        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            var toggleInfo = new RequestFilterToggleMetaData
            {
                Tags = Tags,
                ToggleDisabledFilterType = ToggleDisabledFilterType,
                ToggleEnabledFilterType = _toggleEnabledFilterType,
                ToggleName = Name
            };

            builder.AddFilterSelector(new RequestFilterToggleFilterSelector(toggleInfo));
        }
    }
}