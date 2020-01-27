using System;
using System.Collections.Generic;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestFilterToggleInfo
    {
        public IEnumerable<string> Tags { get; internal set; }
        public Type ToggleDisabledFilterType { get; internal set; }
        public Type ToggleEnabledFilterType { get; internal set; }
        public string ToggleName { get; internal set; }
    }
}