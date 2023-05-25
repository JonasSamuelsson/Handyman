using System;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.EventFilterToggle
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

        public string? Name { get; set; }
        public string[]? Tags { get; set; }
        public Type[]? ToggleDisabledFilterTypes { get; set; }
        public ToggleFailureMode? FailureMode { get; set; }

        public override async Task Execute<TEvent>(EventPipelineBuilderContext<TEvent> pipelineBuilderContext, EventContext<TEvent> eventContext)
        {
            var metadata = _metadata.Value;
            var toggle = eventContext.ServiceProvider.GetRequiredService<IEventFilterToggle>();
            var toggleState = await toggle.IsEnabled(metadata, eventContext).WithGloballyConfiguredAwait();

            PipelineBuilderUtilities.ApplyToggle(pipelineBuilderContext.Filters, _toggleEnabledFilterTypes, ToggleDisabledFilterTypes, toggleState);
        }

        private EventFilterToggleMetadata CreateMetadata()
        {
            return new EventFilterToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledFilterTypes = ToggleDisabledFilterTypes ?? Type.EmptyTypes,
                ToggleEnabledFilterTypes = _toggleEnabledFilterTypes,
                FailureMode = FailureMode
            };
        }
    }
}