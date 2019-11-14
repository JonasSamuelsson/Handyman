using System;

namespace Handyman.Mediator.EventPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventFilterToggleAttribute : EventPipelineBuilderAttribute
    {
        private readonly Type _toggledFilterType;

        public EventFilterToggleAttribute(Type toggledFilterType)
        {
            _toggledFilterType = toggledFilterType ?? throw new ArgumentNullException(nameof(toggledFilterType));
        }

        public Type FallbackFilterType { get; set; }

        public override void Configure<TEvent>(IEventPipelineBuilder<TEvent> builder, ServiceProvider serviceProvider)
        {
            builder.AddFilterSelector(new EventFilterToggleFilterSelector<TEvent>(_toggledFilterType)
            {
                FallbackFilterType = FallbackFilterType
            });
        }
    }
}