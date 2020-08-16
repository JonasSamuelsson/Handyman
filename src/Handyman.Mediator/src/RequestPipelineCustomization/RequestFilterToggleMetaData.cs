using Handyman.Mediator.PipelineCustomization;
using System;
using System.Collections.Generic;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestFilterToggleMetadata : IToggleMetadata
    {
        public string Name { get; internal set; }
        public IEnumerable<string> Tags { get; internal set; }
        public Type[] ToggleDisabledFilterTypes { get; internal set; }
        public Type[] ToggleEnabledFilterTypes { get; internal set; }
    }
}