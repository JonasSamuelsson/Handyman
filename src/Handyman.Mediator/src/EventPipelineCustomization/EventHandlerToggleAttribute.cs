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

        public Type ToggleDisabledHandlerType { get; set; }

        public override void Configure(IEventPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            builder.AddHandlerSelector(new EventHandlerToggleHandlerSelector(_toggleEnabledHandlerType)
            {
                ToggleDisabledHandlerType = ToggleDisabledHandlerType
            });
        }
    }
}