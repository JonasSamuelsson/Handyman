using System;
using System.Collections.Generic;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public class EventFilterToggleMetaData
    {
        public IEnumerable<string> Tags { get; internal set; }
        public Type ToggleDisabledFilterType { get; internal set; }
        public Type ToggleEnabledFilterType { get; internal set; }
        public string ToggleName { get; internal set; }
    }
}