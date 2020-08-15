using System;
using System.Collections.Generic;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public class EventHandlerToggleMetaData
    {
        public IEnumerable<string> Tags { get; internal set; }
        public Type ToggleDisabledHandlerType { get; internal set; }
        public Type ToggleEnabledHandlerType { get; internal set; }
        public string ToggleName { get; internal set; }
    }
}