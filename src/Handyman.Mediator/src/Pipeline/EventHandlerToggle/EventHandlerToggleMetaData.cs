using System;
using System.Collections.Generic;

namespace Handyman.Mediator.Pipeline.EventHandlerToggle
{
    public class EventHandlerToggleMetadata : IToggleMetadata
    {
        public string? Name { get; internal set; }
        public IEnumerable<string>? Tags { get; internal set; }
        public IEnumerable<Type> ToggleDisabledHandlerTypes { get; internal set; } = null!;
        public IEnumerable<Type> ToggleEnabledHandlerTypes { get; internal set; } = null!;
        public ToggleFailureMode? FailureMode { get; set; }
    }
}