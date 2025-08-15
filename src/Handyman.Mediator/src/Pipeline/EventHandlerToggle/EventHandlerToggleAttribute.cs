using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.EventHandlerToggle;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EventHandlerToggleAttribute : EventPipelineBuilderAttribute
{
    private readonly Lazy<EventHandlerToggleMetadata> _metadata;
    private readonly Type[] _toggleEnabledHandlerTypes;

    public EventHandlerToggleAttribute(Type toggleEnabledHandlerType)
        : this(new[] { toggleEnabledHandlerType })
    {
    }

    public EventHandlerToggleAttribute(Type toggleEnabledHandlerType, Type toggleDisabledHandlerType)
        : this(new[] { toggleEnabledHandlerType }, new[] { toggleDisabledHandlerType })
    {
    }

    public EventHandlerToggleAttribute(Type toggleEnabledHandlerType, Type[] toggleDisabledHandlerTypes)
        : this(new[] { toggleEnabledHandlerType }, toggleDisabledHandlerTypes)
    {
    }

    public EventHandlerToggleAttribute(Type[] toggleEnabledHandlerTypes)
    {
        _metadata = new Lazy<EventHandlerToggleMetadata>(CreateMetadata);
        _toggleEnabledHandlerTypes = toggleEnabledHandlerTypes;
    }

    public EventHandlerToggleAttribute(Type[] toggleEnabledHandlerTypes, Type toggleDisabledHandlerType)
        : this(toggleEnabledHandlerTypes, new[] { toggleDisabledHandlerType })
    {
    }

    public EventHandlerToggleAttribute(Type[] toggleEnabledHandlerTypes, Type[] toggleDisabledHandlerTypes)
        : this(toggleEnabledHandlerTypes)
    {
        ToggleDisabledHandlerTypes = toggleDisabledHandlerTypes;
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

#if NET8_0_OR_GREATER

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EventHandlerToggleAttribute<TToggleEnabledHandler> : EventHandlerToggleAttribute
{
    public EventHandlerToggleAttribute() : base(typeof(TToggleEnabledHandler))
    {
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EventHandlerToggleAttribute<TToggleEnabledHandler, TToggleDisabledHandler> : EventHandlerToggleAttribute
{
    public EventHandlerToggleAttribute() : base(typeof(TToggleEnabledHandler), typeof(TToggleDisabledHandler))
    {
    }
}

#endif