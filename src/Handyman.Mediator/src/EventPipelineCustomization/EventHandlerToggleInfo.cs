using System;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public class EventHandlerToggleInfo
    {
        public Type ToggleDisabledHandlerType { get; internal set; }
        public Type ToggleEnabledHandlerType { get; internal set; }
        public string ToggleName { get; internal set; }
    }
}