using System;

namespace Handyman.Mediator.EventPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventFilterToggleAttribute : EventPipelineBuilderAttribute
    {
        private readonly Type _toggleEnabledFilterType;

        public EventFilterToggleAttribute(Type toggleEnabledFilterType)
        {
            _toggleEnabledFilterType = toggleEnabledFilterType ?? throw new ArgumentNullException(nameof(toggleEnabledFilterType));
        }

        public Type ToggleDisabledFilterType { get; set; }
        public string ToggleName { get; set; }

        public override void Configure(IEventPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            var toggleInfo = new EventFilterToggleInfo
            {
                ToggleDisabledFilterType = ToggleDisabledFilterType,
                ToggleEnabledFilterType = _toggleEnabledFilterType,
                ToggleName = ToggleName
            };

            builder.AddFilterSelector(new EventFilterToggleFilterSelector(toggleInfo));
        }
    }
}