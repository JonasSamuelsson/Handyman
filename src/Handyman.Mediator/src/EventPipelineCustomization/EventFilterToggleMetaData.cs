using Handyman.Mediator.PipelineCustomization;
using System;
using System.Collections.Generic;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public class EventFilterToggleMetadata : IToggleMetadata
    {
        public string Name { get; internal set; }
        public IEnumerable<string> Tags { get; internal set; }
        public Type ToggleDisabledFilterType { get; internal set; }
        public Type ToggleEnabledFilterType { get; internal set; }
    }
}