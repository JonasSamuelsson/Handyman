using Handyman.Mediator.PipelineCustomization;
using System;
using System.Collections.Generic;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public class EventHandlerToggleMetadata : IToggleMetadata
    {
        public string Name { get; internal set; }
        public IEnumerable<string> Tags { get; internal set; }
        public IEnumerable<Type> ToggleDisabledHandlerTypes { get; internal set; }
        public IEnumerable<Type> ToggleEnabledHandlerTypes { get; internal set; }
    }
}