using System;
using System.Collections.Generic;

namespace Handyman.Mediator.Pipeline.RequestHandlerToggle
{
    public class RequestHandlerToggleMetadata : IToggleMetadata
    {
        public string? Name { get; internal set; }
        public IEnumerable<string>? Tags { get; internal set; }
        public IEnumerable<Type> ToggleDisabledHandlerTypes { get; internal set; } = null!;
        public IEnumerable<Type> ToggleEnabledHandlerTypes { get; internal set; } = null!;
    }
}