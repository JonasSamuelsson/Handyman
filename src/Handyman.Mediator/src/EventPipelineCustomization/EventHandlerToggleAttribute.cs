using System;
using System.Linq;

namespace Handyman.Mediator.EventPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventHandlerToggleAttribute : EventPipelineBuilderAttribute
    {
        private readonly Lazy<EventHandlerToggleMetadata> _metadata;
        private readonly Type[] _toggleEnabledHandlerTypes;

        public EventHandlerToggleAttribute(Type[] toggleEnabledHandlerTypes)
        {
            if (toggleEnabledHandlerTypes == null)
                throw new ArgumentNullException(nameof(toggleEnabledHandlerTypes));

            if (!toggleEnabledHandlerTypes.Any())
                throw new ArgumentException();

            _metadata = new Lazy<EventHandlerToggleMetadata>(CreateMetadata);
            _toggleEnabledHandlerTypes = toggleEnabledHandlerTypes;
        }

        public string Name { get; set; }
        public string[] Tags { get; set; }
        public Type[] ToggleDisabledHandlerTypes { get; set; }

        public override void Configure(IEventPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            builder.AddHandlerSelector(new EventHandlerToggleHandlerSelector(_metadata.Value));
        }

        private EventHandlerToggleMetadata CreateMetadata()
        {
            return new EventHandlerToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledHandlerTypes = ToggleDisabledHandlerTypes ?? Enumerable.Empty<Type>(),
                ToggleEnabledHandlerTypes = _toggleEnabledHandlerTypes ?? Enumerable.Empty<Type>()
            };
        }
    }
}