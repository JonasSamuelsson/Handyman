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

        public string[] Tags { get; set; } = { };
        public Type ToggleDisabledHandlerType { get; set; }
        public string ToggleName { get; set; }

        public override void Configure(IEventPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            var toggleInfo = new EventHandlerToggleInfo
            {
                Tags = Tags,
                ToggleDisabledHandlerType = ToggleDisabledHandlerType,
                ToggleEnabledHandlerType = _toggleEnabledHandlerType,
                ToggleName = ToggleName
            };

            builder.AddHandlerSelector(new EventHandlerToggleHandlerSelector(toggleInfo));
        }
    }
}