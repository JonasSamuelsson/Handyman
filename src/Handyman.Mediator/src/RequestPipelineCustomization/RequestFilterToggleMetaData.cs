using System;
using System.Collections.Generic;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestFilterToggleMetaData
    {
        public string Name { get; internal set; }
        public IEnumerable<string> Tags { get; internal set; }
        public Type ToggleDisabledFilterType { get; internal set; }
        public Type ToggleEnabledFilterType { get; internal set; }
    }
}