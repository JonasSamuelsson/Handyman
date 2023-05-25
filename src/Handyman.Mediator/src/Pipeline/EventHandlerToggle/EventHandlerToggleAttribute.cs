using System;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.EventHandlerToggle
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventHandlerToggleAttribute : EventPipelineBuilderAttribute
    {
        private readonly Lazy<EventHandlerToggleMetadata> _metadata;
        private readonly Type[] _toggleEnabledHandlerTypes;

        public EventHandlerToggleAttribute(Type toggleEnabledHandlerType)
            : this(new[] { toggleEnabledHandlerType ?? throw new ArgumentNullException(nameof(toggleEnabledHandlerType)) })
        {
        }

        public EventHandlerToggleAttribute(Type[] toggleEnabledHandlerTypes)
        {
            if (toggleEnabledHandlerTypes == null)
                throw new ArgumentNullException(nameof(toggleEnabledHandlerTypes));

            if (!toggleEnabledHandlerTypes.Any())
                throw new ArgumentException();

            _metadata = new Lazy<EventHandlerToggleMetadata>(CreateMetadata);
            _toggleEnabledHandlerTypes = toggleEnabledHandlerTypes;
        }

        public string? Name { get; set; }
        public string[]? Tags { get; set; }
        public Type[]? ToggleDisabledHandlerTypes { get; set; }
        public ToggleFailureMode? FailureMode { get; set; }

        public override async Task Execute<TEvent>(EventPipelineBuilderContext<TEvent> pipelineBuilderContext, EventContext<TEvent> eventContext)
        {
            var metadata = _metadata.Value;
            var toggle = eventContext.ServiceProvider.GetRequiredService<IEventHandlerToggle>();
            var toggleState = await toggle.IsEnabled(metadata, eventContext).WithGloballyConfiguredAwait();

            PipelineBuilderUtilities.ApplyToggle(pipelineBuilderContext.Handlers, _toggleEnabledHandlerTypes, ToggleDisabledHandlerTypes, toggleState);
        }

        private EventHandlerToggleMetadata CreateMetadata()
        {
            return new EventHandlerToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledHandlerTypes = ToggleDisabledHandlerTypes ?? Type.EmptyTypes,
                ToggleEnabledHandlerTypes = _toggleEnabledHandlerTypes,
                FailureMode = FailureMode
            };
        }
    }
}