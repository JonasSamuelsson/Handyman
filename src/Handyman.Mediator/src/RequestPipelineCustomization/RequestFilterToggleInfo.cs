using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestFilterToggleInfo
    {
        public Type ToggleDisabledFilterType { get; internal set; }
        public Type ToggleEnabledFilterType { get; internal set; }
        public string ToggleName { get; internal set; }
    }
}