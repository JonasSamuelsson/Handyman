using System;

namespace Handyman.Mediator.EventPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventHandlerToggleAttribute : EventPipelineBuilderAttribute
    {
        private readonly Type _toggleEnabledHandlerType;

        public EventHandlerToggleAttribute(Type toggleEnabledHandlerType)
        {
            _toggleEnabledHandlerType = toggleEnabledHandlerType ?? throw new ArgumentNullException(nameof(toggleEnabledHandlerType));
        }

        public string Name { get; set; }
        public string[] Tags { get; set; }
        public Type ToggleDisabledHandlerType { get; set; }

        public override void Configure(IEventPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            var toggleInfo = new EventHandlerToggleMetaData
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledHandlerType = ToggleDisabledHandlerType,
                ToggleEnabledHandlerType = _toggleEnabledHandlerType
            };

            builder.AddHandlerSelector(new EventHandlerToggleHandlerSelector(toggleInfo));
        }
    }
}