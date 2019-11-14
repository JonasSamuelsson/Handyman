using System;

namespace Handyman.Mediator.EventPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventHandlerToggleAttribute : EventPipelineBuilderAttribute
    {
        private readonly Type _toggledHandlerType;

        public EventHandlerToggleAttribute(Type toggledHandlerType)
        {
            _toggledHandlerType = toggledHandlerType ?? throw new ArgumentNullException(nameof(toggledHandlerType));
        }

        public Type FallbackHandlerType { get; set; }

        public override void Configure<TEvent>(IEventPipelineBuilder<TEvent> builder, ServiceProvider serviceProvider)
        {
            builder.AddHandlerSelector(new EventHandlerToggleHandlerSelector<TEvent>(_toggledHandlerType)
            {
                FallbackHandlerType = FallbackHandlerType
            });
        }
    }
}