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

        public string Name { get; set; }
        public string[] Tags { get; set; }
        public Type ToggleDisabledFilterType { get; set; }

        public override void Configure(IEventPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            var toggleInfo = new EventFilterToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledFilterType = ToggleDisabledFilterType,
                ToggleEnabledFilterType = _toggleEnabledFilterType
            };

            builder.AddFilterSelector(new EventFilterToggleFilterSelector(toggleInfo));
        }
    }
}