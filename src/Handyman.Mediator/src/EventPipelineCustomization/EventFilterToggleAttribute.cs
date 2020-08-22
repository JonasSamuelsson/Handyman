using System;
using System.Linq;

namespace Handyman.Mediator.EventPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventFilterToggleAttribute : EventPipelineBuilderAttribute
    {
        private readonly Lazy<EventFilterToggleMetadata> _metadata;
        private readonly Type[] _toggleEnabledFilterTypes;

        public EventFilterToggleAttribute(Type toggleEnabledFilterType)
            : this(new[] { toggleEnabledFilterType ?? throw new ArgumentNullException(nameof(toggleEnabledFilterType)) })
        {
        }

        public EventFilterToggleAttribute(Type[] toggleEnabledFilterTypes)
        {
            if (toggleEnabledFilterTypes == null)
                throw new ArgumentNullException(nameof(toggleEnabledFilterTypes));

            if (!toggleEnabledFilterTypes.Any())
                throw new ArgumentException();

            _metadata = new Lazy<EventFilterToggleMetadata>(CreateMetadata);
            _toggleEnabledFilterTypes = toggleEnabledFilterTypes;
        }

        public string Name { get; set; }
        public string[] Tags { get; set; }
        public Type[] ToggleDisabledFilterTypes { get; set; }

        public override void Configure(IEventPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            builder.AddFilterSelector(new EventFilterToggleFilterSelector(_metadata.Value));
        }

        private EventFilterToggleMetadata CreateMetadata()
        {
            return new EventFilterToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledFilterTypes = ToggleDisabledFilterTypes ?? Enumerable.Empty<Type>(),
                ToggleEnabledFilterTypes = _toggleEnabledFilterTypes ?? Enumerable.Empty<Type>()
            };
        }
    }
}