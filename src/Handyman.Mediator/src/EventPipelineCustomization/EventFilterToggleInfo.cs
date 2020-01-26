using System;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public class EventFilterToggleInfo
    {
        public Type ToggleDisabledFilterType { get; internal set; }
        public Type ToggleEnabledFilterType { get; internal set; }
        public string ToggleName { get; internal set; }
    }
}