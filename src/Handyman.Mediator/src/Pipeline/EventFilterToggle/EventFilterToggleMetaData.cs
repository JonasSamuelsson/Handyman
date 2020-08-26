using System;
using System.Collections.Generic;

namespace Handyman.Mediator.Pipeline.EventFilterToggle
{
    public class EventFilterToggleMetadata : IToggleMetadata
    {
        public string Name { get; internal set; }
        public IEnumerable<string> Tags { get; internal set; }
        public IEnumerable<Type> ToggleDisabledFilterTypes { get; internal set; }
        public IEnumerable<Type> ToggleEnabledFilterTypes { get; internal set; }
    }
}