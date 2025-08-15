using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.EventFilterToggle;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EventFilterToggleAttribute : EventPipelineBuilderAttribute
{
    private readonly Lazy<EventFilterToggleMetadata> _metadata;
    private readonly Type[] _toggleEnabledFilterTypes;

    public EventFilterToggleAttribute(Type toggleEnabledFilterType)
        : this(new[] { toggleEnabledFilterType })
    {
    }

    public EventFilterToggleAttribute(Type toggleEnabledFilterType, Type toggleDisabledFilterType)
        : this(new[] { toggleEnabledFilterType }, new[] { toggleDisabledFilterType })
    {
    }

    public EventFilterToggleAttribute(Type toggleEnabledFilterType, Type[] toggleDisabledFilterTypes)
        : this(new[] { toggleEnabledFilterType }, toggleDisabledFilterTypes)
    {
    }

    public EventFilterToggleAttribute(Type[] toggleEnabledFilterTypes)
    {
        _metadata = new Lazy<EventFilterToggleMetadata>(CreateMetadata);
        _toggleEnabledFilterTypes = toggleEnabledFilterTypes;
    }

    public EventFilterToggleAttribute(Type[] toggleEnabledFilterTypes, Type toggleDisabledFilterType)
        : this(toggleEnabledFilterTypes, new[] { toggleDisabledFilterType })
    {
    }

    public EventFilterToggleAttribute(Type[] toggleEnabledFilterTypes, Type[] toggleDisabledFilterTypes)
        : this(toggleEnabledFilterTypes)
    {
        ToggleDisabledFilterTypes = toggleDisabledFilterTypes;
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

#if NET8_0_OR_GREATER

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EventFilterToggleAttribute<TToggleEnabledFilter> : EventFilterToggleAttribute
{
    public EventFilterToggleAttribute() : base(typeof(TToggleEnabledFilter))
    {
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EventFilterToggleAttribute<TToggleEnabledFilter, TToggleDisabledFilter> : EventFilterToggleAttribute
{
    public EventFilterToggleAttribute() : base(typeof(TToggleEnabledFilter), typeof(TToggleDisabledFilter))
    {
    }
}

#endif